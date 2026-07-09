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

        public async Task<IEnumerable<MedicineDto>> GetAllMedicines(Guid pharmacyId, string? search = null, string? category = null)
        {
            var prices = await _unitOfWork.PharmacyMedicineRepository.Search(pharmacyId, search, category);
            return prices.Select(p => p.ToDto());
        }

        public async Task<MedicineDto?> GetMedicineById(Guid pharmacyId, Guid id)
        {
            var price = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(id, pharmacyId);
            return price?.ToDto();
        }

        public async Task<MedicineStatsDto> GetStats(Guid pharmacyId)
        {
            var prices = (await _unitOfWork.PharmacyMedicineRepository.GetAllForPharmacy(pharmacyId)).ToList();
            var active = prices.Count(p => p.Medicine.IsActive);

            return new MedicineStatsDto
            {
                TotalMedicines = prices.Count,
                Active = active,
                Inactive = prices.Count - active,
                PrescriptionRequired = prices.Count(p => p.Medicine.IsPrescriptionRequired),
                Controlled = prices.Count(p => p.Medicine.IsControlled),
                CategoriesCount = prices.Select(p => p.Medicine.Category).Distinct().Count()
            };
        }

        // STEP 1 of the scenario: "Search First"
        public async Task<IEnumerable<GlobalMedicineSearchResultDto>> SearchGlobalCatalog(Guid pharmacyId, string? query)
        {
            var medicines = (await _unitOfWork.MedicineRepository.SearchGlobal(query)).ToList();
            if (medicines.Count == 0) return Enumerable.Empty<GlobalMedicineSearchResultDto>();

            var pharmacyMedicines = await _unitOfWork.PharmacyMedicineRepository.GetAllForPharmacy(pharmacyId);
            var linkedIds = pharmacyMedicines.Select(p => p.MedicineId).ToHashSet();

            return medicines.Select(m => m.ToSearchResultDto(linkedIds.Contains(m.Id)));
        }

        // STEP 2 of the scenario: "Existing Medicine Found" -> "Add to Pharmacy"
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

            var price = new PharmacyMedicine
            {
                Id = Guid.NewGuid(),
                MedicineId = medicine.Id,
                PharmacyId = pharmacyId,
                TaxId = dto.TaxId,
                PurchasePrice = dto.PurchasePrice,
                SellingPrice = dto.SellingPrice,
                MinStockLevel = dto.MinStockLevel,
                ChangedAt = DateTime.UtcNow,
            };

            _unitOfWork.PharmacyMedicineRepository.Add(price);
            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(medicine.Id, pharmacyId);
            return new LinkExistingResult { Medicine = saved!.ToDto() };
        }

        // STEP 3 of the scenario: "Medicine Not Found" -> "Create & Add to Pharmacy"
        public async Task<MedicineCreateResult> CreateMedicine(Guid pharmacyId, MedicineCreateDto dto)
        {
            var existing = await _unitOfWork.MedicineRepository.GetByTradeNameEn(dto.TradeNameEn);
            if (existing is not null)
            {
                // Safety net: don't silently attach. Tell the caller to use LinkExistingMedicine instead.
                return new MedicineCreateResult { ExistingMedicineFound = true, ExistingMedicineId = existing.Id };
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
                TaxId = dto.TaxId,
                PurchasePrice = dto.PurchasePrice,
                SellingPrice = dto.SellingPrice,
                MinStockLevel = dto.MinStockLevel,
                ChangedAt = DateTime.UtcNow,
            };
            _unitOfWork.PharmacyMedicineRepository.Add(price);

            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(medicine.Id, pharmacyId);
            return new MedicineCreateResult { Medicine = saved!.ToDto() };
        }

        // Pharmacist edit: pharmacy-specific fields ONLY. Global data is untouched.
        public async Task<MedicineUpdateResult> UpdatePharmacyMedicine(Guid pharmacyId, Guid id, PharmacyMedicineUpdateDto dto)
        {
            var price = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(id, pharmacyId);
            if (price is null)
            {
                return new MedicineUpdateResult { NotFound = true };
            }

            dto.ApplyTo(price);
            price.ChangedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.PharmacyMedicineRepository.GetByMedicineAndPharmacy(id, pharmacyId);
            return new MedicineUpdateResult { Medicine = saved!.ToDto() };
        }

        // Admin-only edit: global catalog data.
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

            price.Medicine.IsActive = !price.Medicine.IsActive;
            price.Medicine.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();
            return price.ToDto();
        }
    }
}