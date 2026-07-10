namespace SafePharma.DAL
{
    public interface IPharmacyMedicineRepository : IGenircRepository<PharmacyMedicine>
    {
        Task<PharmacyMedicine?> GetByMedicineAndPharmacy(Guid medicineId, Guid pharmacyId);
        Task<PharmacyMedicine?> GetDetailsByMedicineAndPharmacy(Guid medicineId, Guid pharmacyId);
        Task<IEnumerable<PharmacyMedicine>> Search(Guid pharmacyId, string? query, string? category = null, bool includeInactive = false);
        Task<IEnumerable<PharmacyMedicine>> GetAllForPharmacy(Guid pharmacyId);
    }
}