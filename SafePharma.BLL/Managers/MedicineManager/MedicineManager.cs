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
            var prices = await _unitOfWork.MedicinePriceRepository.Search(pharmacyId, search, category);
            return prices.Select(p => p.ToDto());
        }

        public async Task<MedicineDto?> GetMedicineById(Guid pharmacyId, Guid id)
        {
            var price = await _unitOfWork.MedicinePriceRepository.GetByMedicineAndPharmacy(id, pharmacyId);
            return price?.ToDto();
        }

        public async Task<MedicineStatsDto> GetStats(Guid pharmacyId)
        {
            var prices = (await _unitOfWork.MedicinePriceRepository.GetAllForPharmacy(pharmacyId)).ToList();
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

        public async Task<MedicineCreateResult> CreateMedicine(Guid pharmacyId, MedicineCreateDto dto)
        {
            var medicine = await _unitOfWork.MedicineRepository.GetByTradeNameEn(dto.TradeNameEn);

            if (medicine is not null)
            {
                var existingPrice = await _unitOfWork.MedicinePriceRepository.GetByMedicineAndPharmacy(medicine.Id, pharmacyId);
                if (existingPrice is not null)
                {
                    return new MedicineCreateResult { DuplicateTradeName = true };
                }
            }
            else
            {
                medicine = dto.ToMedicineEntity();
                medicine.Id = Guid.NewGuid();
                medicine.CreatedAt = DateTime.UtcNow;
                medicine.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.MedicineRepository.Add(medicine);
            }

            var price = new MedicinePrice
            {
                Id = Guid.NewGuid(),
                MedicineId = medicine.Id,
                PharmacyId = pharmacyId,
                TaxId = dto.TaxId,
                PurchasePrice = dto.PurchasePrice,
                SellingPrice = dto.SellingPrice,
                ChangedAt = DateTime.UtcNow,
            };

            _unitOfWork.MedicinePriceRepository.Add(price);
            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.MedicinePriceRepository.GetByMedicineAndPharmacy(medicine.Id, pharmacyId);
            return new MedicineCreateResult { Medicine = saved!.ToDto() };
        }

        public async Task<MedicineUpdateResult> UpdateMedicine(Guid pharmacyId, Guid id, MedicineUpdateDto dto)
        {
            var price = await _unitOfWork.MedicinePriceRepository.GetByMedicineAndPharmacy(id, pharmacyId);
            if (price is null)
            {
                return new MedicineUpdateResult { NotFound = true };
            }

            if (await _unitOfWork.MedicineRepository.TradeNameExists(dto.TradeNameEn, id))
            {
                return new MedicineUpdateResult { DuplicateTradeName = true };
            }

            dto.ApplyTo(price.Medicine, price);
            price.Medicine.UpdatedAt = DateTime.UtcNow;
            price.ChangedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.MedicinePriceRepository.GetByMedicineAndPharmacy(id, pharmacyId);
            return new MedicineUpdateResult { Medicine = saved!.ToDto() };
        }

        public async Task<bool> DeleteMedicine(Guid pharmacyId, Guid id)
        {
            var price = await _unitOfWork.MedicinePriceRepository.GetByMedicineAndPharmacy(id, pharmacyId);
            if (price is null)
            {
                return false;
            }

            _unitOfWork.MedicinePriceRepository.Delete(price);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<MedicineDto?> ToggleStatus(Guid pharmacyId, Guid id)
        {
            var price = await _unitOfWork.MedicinePriceRepository.GetByMedicineAndPharmacy(id, pharmacyId);
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