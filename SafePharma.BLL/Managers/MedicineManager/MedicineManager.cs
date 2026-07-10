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
            var price = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(id, pharmacyId);
            if (price is null) return null;

            var aggregates = (await _unitOfWork._batchRepository.GetStockAggregates(new[] { price.Id })).ToList();
            var agg = aggregates.FirstOrDefault();
            return price.ToDto(agg?.AvailableQuantity ?? 0, agg?.BatchCount ?? 0);
        }

        public async Task<MedicineStatsDto> GetStats(Guid pharmacyId)
        {
            var prices = (await _unitOfWork.PharmacyMedicineRepository.GetAllForPharmacy(pharmacyId)).ToList();
            var active = prices.Count(p => p.Medicine.IsActive && p.IsActive);

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
                PrescriptionRequired = prices.Count(p => p.Medicine.IsPrescriptionRequired),
                Controlled = prices.Count(p => p.Medicine.IsControlled),
                CategoriesCount = prices.Select(p => p.Medicine.Category).Distinct().Count(),
                BelowMinStock = belowMinStock,
            };
        }

        public async Task<IEnumerable<GlobalMedicineSearchResultDto>> SearchGlobalCatalog(Guid pharmacyId, string? query)
        {
            var medicines = (await _unitOfWork.MedicineRepository.SearchGlobal(query)).ToList();
            if (medicines.Count == 0) return Enumerable.Empty<GlobalMedicineSearchResultDto>();

            var pharmacyMedicines = await _unitOfWork.PharmacyMedicineRepository.GetAllForPharmacy(pharmacyId);
            var linkedIds = pharmacyMedicines.Select(p => p.MedicineId).ToHashSet();

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

        public async Task<LinkExistingResult> LinkExistingMedicine(Guid pharmacyId, LinkExistingMedicineDto dto)
        {
            var medicine = await _unitOfWork.MedicineRepository.GetById(dto.MedicineId);
            if (medicine is null)
            {
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

            var price = new PharmacyMedicine
            {
                Id = Guid.NewGuid(),
                MedicineId = medicine.Id,
                PharmacyId = pharmacyId,
                PurchasePrice = dto.PurchasePrice,
                SellingPrice = dto.SellingPrice,
                MinStockLevel = dto.MinStockLevel,
                ChangedAt = DateTime.UtcNow,
            };
            foreach (var link in taxLinks)
            {
                link.PharmacyMedicineId = price.Id;
                price.PharmacyMedicineTaxes.Add(link);
            }

            _unitOfWork.PharmacyMedicineRepository.Add(price);
            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(medicine.Id, pharmacyId);
            return new LinkExistingResult { Medicine = saved!.ToDto() };
        }

        public async Task<MedicineCreateResult> CreateMedicine(Guid pharmacyId, MedicineCreateDto dto)
        {
            var existing = await _unitOfWork.MedicineRepository.GetByTradeNameEn(dto.TradeNameEn);
            if (existing is not null)
            {
                return new MedicineCreateResult { ExistingMedicineFound = true, ExistingMedicineId = existing.Id };
            }

            var taxLinks = await BuildTaxLinksAsync(pharmacyId, dto.TaxIds);
            if (taxLinks is null)
            {
                return new MedicineCreateResult { InvalidTaxIds = true };
            }

            var medicine = dto.ToMedicineEntity();
            medicine.Id = Guid.NewGuid();
            medicine.CreatedAt = DateTime.UtcNow;
            medicine.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.MedicineRepository.Add(medicine);

            var price = new PharmacyMedicine
            {
                Id = Guid.NewGuid(),
                MedicineId = medicine.Id,
                PharmacyId = pharmacyId,
                PurchasePrice = dto.PurchasePrice,
                SellingPrice = dto.SellingPrice,
                MinStockLevel = dto.MinStockLevel,
                SKU = dto.SKU,
                ChangedAt = DateTime.UtcNow,
            };
            foreach (var link in taxLinks)
            {
                link.PharmacyMedicineId = price.Id;
                price.PharmacyMedicineTaxes.Add(link);
            }
            _unitOfWork.PharmacyMedicineRepository.Add(price);

            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(medicine.Id, pharmacyId);
            return new MedicineCreateResult { Medicine = saved!.ToDto() };
        }

        public async Task<MedicineUpdateResult> UpdatePharmacyMedicine(Guid pharmacyId, Guid id, PharmacyMedicineUpdateDto dto)
        {
            var price = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(id, pharmacyId);
            if (price is null)
            {
                return new MedicineUpdateResult { NotFound = true };
            }

            var taxLinks = await BuildTaxLinksAsync(pharmacyId, dto.TaxIds);
            if (taxLinks is null)
            {
                return new MedicineUpdateResult { InvalidTaxIds = true };
            }

            dto.ApplyTo(price);
            price.ChangedAt = DateTime.UtcNow;

            // Replace the tax set entirely (tracked entity, so EF diffs the collection on save).
            price.PharmacyMedicineTaxes.Clear();
            foreach (var link in taxLinks)
            {
                link.PharmacyMedicineId = price.Id;
                price.PharmacyMedicineTaxes.Add(link);
            }

            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(id, pharmacyId);
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
            var price = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(id, pharmacyId);
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
            var price = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(id, pharmacyId);
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
            var price = await _unitOfWork.PharmacyMedicineRepository.GetDetailsByMedicineAndPharmacy(id, pharmacyId);
            if (price is null)
            {
                return null;
            }

            var batches = await _unitOfWork._batchRepository.GetBatchesByhMedicineId(price.Id);
            return price.ToDetailsDto(batches);
        }
    }
}