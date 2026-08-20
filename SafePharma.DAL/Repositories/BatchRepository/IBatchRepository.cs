namespace SafePharma.DAL
{
    public interface IBatchRepository : IGenircRepository<Batch>
    {
        Task<IEnumerable<IGrouping<Guid, Batch>>> GetBatchesGroupByMedicineAsync(Guid pharmacyId);
        Task<IEnumerable<Batch>> GetBatchesByMedicineId(Guid medicineId);

        Task<Batch?> GetByIdForPharmacyAsync(
                                                Guid batchId,
                                                Guid pharmacyId);
 
       
        Task<Batch?> GetByPurchaseReceiptItemId(Guid purchaseReceiptItemId);
        Task<IEnumerable<StockAggregate>> GetStockAggregates(IEnumerable<Guid> pharmacyMedicineIds, int expiringSoonDays = 90);
        Task<IEnumerable<Batch>> GetBatchesForExpiryNotifications();

        Task<int> GetAvailableQuantity(Guid pharmacyMedicineId, Guid pharmacyId);

        Task<Batch?> GetNearestExpiryBatchAsync(Guid pharmacyMedicineId);
    }
}