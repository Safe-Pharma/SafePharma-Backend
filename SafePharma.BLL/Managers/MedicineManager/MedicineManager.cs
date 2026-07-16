using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class MedicineManager : IMedicineManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicineManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private const string SkuPrefix = "RX-";
        private const int SkuStartNumber = 1001;

        private readonly record struct SkuResolution(string? Sku, bool Duplicate)
        {
            public static SkuResolution Ok(string sku) => new(sku, false);
            public static SkuResolution AsDuplicate() => new(null, true);
        }

        // Single source of truth for SKU behavior across Create, Local Create, Link, and Update.
        // - blank/null + no fallback  -> auto-generate next number for this pharmacy
        // - blank/null + fallback     -> keep the current SKU unchanged (used on Update)
        // - provided                  -> validate uniqueness within the pharmacy (excluding self on Update)
        private async Task<SkuResolution> ResolveSkuAsync(
            Guid pharmacyId,
            string? requestedSku,
            Guid? excludePharmacyMedicineId = null,
            string? fallbackSku = null)
        {
            var trimmed = requestedSku?.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                if (!string.IsNullOrWhiteSpace(fallbackSku))
                {
                    return SkuResolution.Ok(fallbackSku);
                }

                var highest = await _unitOfWork.PharmacyMedicineRepository.GetHighestAutoSkuNumber(pharmacyId, SkuPrefix);
                var next = Math.Max(highest + 1, SkuStartNumber);
                return SkuResolution.Ok($"{SkuPrefix}{next}");
            }

            var duplicate = await _unitOfWork.PharmacyMedicineRepository
                .SkuExistsForPharmacy(pharmacyId, trimmed, excludePharmacyMedicineId);

            return duplicate ? SkuResolution.AsDuplicate() : SkuResolution.Ok(trimmed);
        }

        public async Task<IEnumerable<MedicineDto>> GetAllMedicines(Guid pharmacyId, string? search = null, string? category = null, bool includeInactive = false)
        {
            var prices = (await _unitOfWork.PharmacyMedicineRepository.Search(pharmacyId, search, category, includeInactive)).ToList();
            if (prices.Count == 0) return Enumerable.Empty<MedicineDto>();

            var aggregates = (await _unitOfWork._batchRepository.GetStockAggregates(prices.Select(p => p.Id)))
                .ToDictionary(a => a.PharmacyMedicineId);

            return prices.Select(p =>
            {
                aggregates.TryGetValue(p.Id, out var agg);
                return p.ToDto(agg?.AvailableQuantity ?? 0, agg?.BatchCount ?? 0);
            });
        }

        public async Task<MedicineDto?> GetMedicineById(Guid pharmacyId, Guid id)
        {
            // `id` is the PharmacyMedicine.Id — the canonical identifier now, since
            // local (non-imported) medicines have no global Medicine.Id at all.
            var price = await _unitOfWork.PharmacyMedicineRepository.GetByIdAndPharmacy(id, pharmacyId, includeDetails: true);
            if (price is null) return null;

            var aggregates = (await _unitOfWork._batchRepository.GetStockAggregates(new[] { price.Id })).ToList();
            var agg = aggregates.FirstOrDefault();
            return price.ToDto(agg?.AvailableQuantity ?? 0, agg?.BatchCount ?? 0);
        }

        public async Task<MedicineStatsDto> GetStats(Guid pharmacyId)
        {
            var prices = (await _unitOfWork.PharmacyMedicineRepository.GetAllForPharmacy(pharmacyId)).ToList();
            var active = prices.Count(p => p.IsActive);

            var aggregates = (await _unitOfWork._batchRepository.GetStockAggregates(prices.Select(p => p.Id)))
                .ToDictionary(a => a.PharmacyMedicineId);

            var belowMinStock = prices.Count(p =>
            {
                aggregates.TryGetValue(p.Id, out var agg);
                var available = agg?.AvailableQuantity ?? 0;
                return available < p.MinStockLevel;
            });

            return new MedicineStatsDto
            {
                TotalMedicines = prices.Count,
                Active = active,
                Inactive = prices.Count - active,
                PrescriptionRequired = prices.Count(p => p.IsPrescriptionRequired),
                Controlled = prices.Count(p => p.IsControlled),
                CategoriesCount = prices.Select(p => p.Category).Distinct().Count(),
                BelowMinStock = belowMinStock,
            };
        }

        public async Task<IEnumerable<GlobalMedicineSearchResultDto>> SearchGlobalCatalog(Guid pharmacyId, string? query)
        {
            var medicines = (await _unitOfWork.MedicineRepository.SearchGlobal(query)).ToList();
            if (medicines.Count == 0) return Enumerable.Empty<GlobalMedicineSearchResultDto>();

            var pharmacyMedicines = await _unitOfWork.PharmacyMedicineRepository.GetAllForPharmacy(pharmacyId);
            var linkedIds = pharmacyMedicines
                .Where(p => p.MedicineId.HasValue)
                .Select(p => p.MedicineId!.Value)
                .ToHashSet();

            return medicines.Select(m => m.ToSearchResultDto(linkedIds.Contains(m.Id)));
        }

        // Validates every requested tax id exists and belongs to the pharmacy. Returns null if any are invalid.
        private async Task<List<PharmacyMedicineTax>?> BuildTaxLinksAsync(Guid pharmacyId, List<Guid> taxIds)
        {
            var pharmacyTaxes = (await _unitOfWork.TaxRepository.GetAllForPharmacy(pharmacyId))
                .Select(t => t.Id)
                .ToHashSet();

            if (taxIds.Any(id => !pharmacyTaxes.Contains(id)))
            {
                return null;
            }

            return taxIds.Distinct().Select(id => new PharmacyMedicineTax { TaxId = id }).ToList();
        }

        // STEP 2: global medicine found by the pharmacist -> import it into this pharmacy's
        // catalog. Descriptive fields are copied onto the new PharmacyMedicine row so the
        // pharmacy's own list is self-contained (no join needed to read/search it later).
        public async Task<LinkExistingResult> LinkExistingMedicine(Guid pharmacyId, LinkExistingMedicineDto dto)
        {
            var medicine = await _unitOfWork.MedicineRepository.GetById(dto.MedicineId);
            if (medicine is null || !medicine.IsGlobal)
            {
                // Not found, or it's another pharmacy's local medicine — indistinguishable from this pharmacy's view.
                return new LinkExistingResult { MedicineNotFound = true };
            }

            var existingLink = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(dto.MedicineId, pharmacyId);
            if (existingLink is not null)
            {
                return new LinkExistingResult { AlreadyLinked = true };
            }

            var taxLinks = await BuildTaxLinksAsync(pharmacyId, dto.TaxIds);
            if (taxLinks is null)
            {
                return new LinkExistingResult { InvalidTaxIds = true };
            }

            var skuResult = await ResolveSkuAsync(pharmacyId, dto.SKU);
            if (skuResult.Duplicate)
            {
                return new LinkExistingResult { DuplicateSku = true };
            }

            var price = new PharmacyMedicine
            {
                Id = Guid.NewGuid(),
                MedicineId = medicine.Id,
                PharmacyId = pharmacyId,
                PurchasePrice = dto.PurchasePrice,
                SellingPrice = dto.SellingPrice,
                MinStockLevel = dto.MinStockLevel,
                SKU = skuResult.Sku!,
                ChangedAt = DateTime.UtcNow,
            };
            medicine.CopyDescriptiveFieldsTo(price);

            foreach (var link in taxLinks)
            {
                link.PharmacyMedicineId = price.Id;
                price.PharmacyMedicineTaxes.Add(link);
            }

            _unitOfWork.PharmacyMedicineRepository.Add(price);

            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (DuplicateSkuException)
            {
                return new LinkExistingResult { DuplicateSku = true };
            }

            var saved = await _unitOfWork.PharmacyMedicineRepository.GetByIdAndPharmacy(price.Id, pharmacyId, includeDetails: true);
            return new LinkExistingResult { Medicine = saved!.ToDto() };
        }

        public async Task<GlobalMedicineCreateResult> CreateGlobalMedicine(GlobalMedicineCreateDto dto)
        {
            var existing = await _unitOfWork.MedicineRepository.GetByTradeNameEn(dto.TradeNameEn);
            if (existing is not null)
            {
                return new GlobalMedicineCreateResult { ExistingMedicineFound = true, ExistingMedicineId = existing.Id };
            }

            var medicine = dto.ToMedicineEntity();
            medicine.Id = Guid.NewGuid();
            medicine.IsGlobal = true;
            medicine.OwnerPharmacyId = null;
            medicine.CreatedAt = DateTime.UtcNow;
            medicine.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.MedicineRepository.Add(medicine);
            await _unitOfWork.SaveAsync();

            return new GlobalMedicineCreateResult { Medicine = medicine.ToGlobalDto() };
        }

        public async Task<MedicineUpdateResult> UpdatePharmacyMedicine(Guid pharmacyId, Guid id, PharmacyMedicineUpdateDto dto)
        {
            var price = await _unitOfWork.PharmacyMedicineRepository.GetByIdAndPharmacy(id, pharmacyId, includeDetails: true);
            if (price is null)
            {
                return new MedicineUpdateResult { NotFound = true };
            }

            var taxLinks = await BuildTaxLinksAsync(pharmacyId, dto.TaxIds);
            if (taxLinks is null)
            {
                return new MedicineUpdateResult { InvalidTaxIds = true };
            }

            var skuResult = await ResolveSkuAsync(pharmacyId, dto.SKU, excludePharmacyMedicineId: price.Id, fallbackSku: price.SKU);
            if (skuResult.Duplicate)
            {
                return new MedicineUpdateResult { DuplicateSku = true };
            }

            dto.ApplyTo(price);
            price.SKU = skuResult.Sku!;
            price.ChangedAt = DateTime.UtcNow;

            price.PharmacyMedicineTaxes.Clear();
            foreach (var link in taxLinks)
            {
                link.PharmacyMedicineId = price.Id;
                price.PharmacyMedicineTaxes.Add(link);
            }

            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (DuplicateSkuException)
            {
                return new MedicineUpdateResult { DuplicateSku = true };
            }

            var saved = await _unitOfWork.PharmacyMedicineRepository.GetByIdAndPharmacy(id, pharmacyId, includeDetails: true);
            return new MedicineUpdateResult { Medicine = saved!.ToDto() };
        }

        public async Task<GlobalMedicineUpdateResult> UpdateGlobalMedicine(Guid id, GlobalMedicineUpdateDto dto)
        {
            var medicine = await _unitOfWork.MedicineRepository.GetById(id);
            if (medicine is null)
            {
                return new GlobalMedicineUpdateResult { NotFound = true };
            }

            if (await _unitOfWork.MedicineRepository.TradeNameExists(dto.TradeNameEn, id))
            {
                return new GlobalMedicineUpdateResult { DuplicateTradeName = true };
            }

            dto.ApplyTo(medicine);
            medicine.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();
            return new GlobalMedicineUpdateResult { Medicine = medicine };
        }

        public async Task<bool> DeleteMedicine(Guid pharmacyId, Guid id)
        {
            var price = await _unitOfWork.PharmacyMedicineRepository.GetByIdAndPharmacy(id, pharmacyId);
            if (price is null)
            {
                return false;
            }

            _unitOfWork.PharmacyMedicineRepository.Delete(price);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<MedicineDto?> ToggleStatus(Guid pharmacyId, Guid id)
        {
            var price = await _unitOfWork.PharmacyMedicineRepository.GetByIdAndPharmacy(id, pharmacyId, includeDetails: true);
            if (price is null)
            {
                return null;
            }

            price.IsActive = !price.IsActive;
            price.ChangedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();
            return price.ToDto();
        }

        public async Task<Medicine?> ToggleGlobalStatus(Guid id)
        {
            var medicine = await _unitOfWork.MedicineRepository.GetById(id);
            if (medicine is null)
            {
                return null;
            }

            medicine.IsActive = !medicine.IsActive;
            medicine.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();
            return medicine;
        }

        public async Task<MedicineDetailsDto?> GetMedicineDetails(Guid pharmacyId, Guid id)
        {
            var price = await _unitOfWork.PharmacyMedicineRepository.GetDetailsByIdAndPharmacy(id, pharmacyId);
            if (price is null)
            {
                return null;
            }

            var aggregates = await _unitOfWork._batchRepository.GetStockAggregates(new[] { price.Id });
            return price.ToDetailsDto(aggregates.FirstOrDefault());
        }

        // STEP 3: not found anywhere in the global catalog -> create a medicine that lives
        // ONLY in this pharmacy's PharmacyMedicine table (MedicineId stays null).
        public async Task<MedicineCreateResult> CreateLocalMedicine(Guid pharmacyId, MedicineCreateDto dto)
        {
            var globalMatch = await _unitOfWork.MedicineRepository.GetByTradeNameEn(dto.TradeNameEn);
            if (globalMatch is not null)
            {
                return new MedicineCreateResult { ExistingMedicineFound = true, ExistingMedicineId = globalMatch.Id };
            }

            if (await _unitOfWork.PharmacyMedicineRepository.TradeNameExistsForPharmacy(pharmacyId, dto.TradeNameEn))
            {
                return new MedicineCreateResult { DuplicateTradeNameInPharmacy = true };
            }

            var taxLinks = await BuildTaxLinksAsync(pharmacyId, dto.TaxIds);
            if (taxLinks is null)
            {
                return new MedicineCreateResult { InvalidTaxIds = true };
            }

            var skuResult = await ResolveSkuAsync(pharmacyId, dto.SKU);
            if (skuResult.Duplicate)
            {
                return new MedicineCreateResult { DuplicateSku = true };
            }

            var price = dto.ToPharmacyMedicineEntity();
            price.Id = Guid.NewGuid();
            price.PharmacyId = pharmacyId;
            price.MedicineId = null;
            price.SKU = skuResult.Sku!;
            price.ChangedAt = DateTime.UtcNow;
            price.CreatedAt = DateTime.UtcNow;
            price.UpdatedAt = DateTime.UtcNow;

            foreach (var link in taxLinks)
            {
                link.PharmacyMedicineId = price.Id;
                price.PharmacyMedicineTaxes.Add(link);
            }
            _unitOfWork.PharmacyMedicineRepository.Add(price);

            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (DuplicateSkuException)
            {
                return new MedicineCreateResult { DuplicateSku = true };
            }

            var saved = await _unitOfWork.PharmacyMedicineRepository.GetByIdAndPharmacy(price.Id, pharmacyId, includeDetails: true);
            return new MedicineCreateResult { Medicine = saved!.ToDto() };
        }

    }
}
