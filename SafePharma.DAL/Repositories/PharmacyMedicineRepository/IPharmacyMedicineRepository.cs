namespace SafePharma.DAL
{
    public interface IPharmacyMedicineRepository : IGenircRepository<PharmacyMedicine>
    {
        // Used to check "did this pharmacy already link this global medicine".
        Task<PharmacyMedicine?> GetByMedicineAndPharmacy(Guid medicineId, Guid pharmacyId);

        // Primary lookups: PharmacyMedicine.Id is the canonical id now (works for both
        // linked and local-only records, since local ones have no MedicineId).
        Task<PharmacyMedicine?> GetByIdAndPharmacy(Guid pharmacyMedicineId, Guid pharmacyId, bool includeDetails = false);
        Task<PharmacyMedicine?> GetDetailsByIdAndPharmacy(Guid pharmacyMedicineId, Guid pharmacyId);

        Task<IEnumerable<PharmacyMedicine>> Search(Guid pharmacyId, string? query, string? category = null, bool includeInactive = false);
        Task<IEnumerable<PharmacyMedicine>> GetAllForPharmacy(Guid pharmacyId);
        Task<int> GetHighestAutoSkuNumber(Guid pharmacyId, string prefix);
        Task<bool> SkuExistsForPharmacy(Guid pharmacyId, string sku, Guid? excludeId = null);
        Task<bool> TradeNameExistsForPharmacy(Guid pharmacyId, string tradeNameEn, Guid? excludeId = null);
        // Paginated search used by MedicineSearchService — searches this pharmacy's own
        // denormalized fields (name/scientific name/SKU/barcodes), no join needed.
        Task<(IEnumerable<PharmacyMedicine> Items, int TotalCount)> SearchAsync(
            Guid pharmacyId, string? query, int pageNumber, int pageSize);
    }
}
