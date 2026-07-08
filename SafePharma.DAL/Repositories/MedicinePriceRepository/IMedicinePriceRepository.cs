namespace SafePharma.DAL
{
    public interface IMedicinePriceRepository : IGenircRepository<MedicinePrice>
    {
        Task<MedicinePrice?> GetByMedicineAndPharmacy(Guid medicineId, Guid pharmacyId);
        Task<IEnumerable<MedicinePrice>> Search(Guid pharmacyId, string? query, string? category = null);
        Task<IEnumerable<MedicinePrice>> GetAllForPharmacy(Guid pharmacyId);
    }
}