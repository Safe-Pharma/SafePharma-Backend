namespace SafePharma.DAL
{
    public interface IPharmacyMedicineRepository : IGenircRepository<PharmacyMedicine>
    {
        Task<PharmacyMedicine?> GetByMedicineAndPharmacy(Guid medicineId, Guid pharmacyId);
        Task<PharmacyMedicine?> GetDetailsByMedicineAndPharmacy(Guid medicineId, Guid pharmacyId);
        Task<IEnumerable<PharmacyMedicine>> Search(Guid pharmacyId, string? query, string? category = null, bool includeInactive = false);
        Task<IEnumerable<PharmacyMedicine>> GetAllForPharmacy(Guid pharmacyId);
        Task<int> GetHighestAutoSkuNumber(Guid pharmacyId, string prefix);
        Task<bool> SkuExistsForPharmacy(Guid pharmacyId, string sku, Guid? excludeId = null);
        Task<PharmacyMedicine?> GetByIdAndPharmacy(Guid pharmacyMedicineId, Guid pharmacyId);
        Task<(IEnumerable<PharmacyMedicine>, int)> SearchAsync(Guid pharmacyId, string query, int pageNumber, int pageSize);
    }
}