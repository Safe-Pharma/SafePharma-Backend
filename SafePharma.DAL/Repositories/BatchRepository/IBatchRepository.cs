namespace SafePharma.DAL
{
    public interface IBatchRepository : IGenircRepository<Batch>
    {
        Task<IEnumerable<IGrouping<Guid, Batch>>> GetBatchesGroupByhMedicine();
        Task<IEnumerable<Batch>> GetBatchesByhMedicineId(Guid MId);
        Task<Batch?> GetByPurchaseReceiptItemId(Guid purchaseReceiptItemId);
        Task<IEnumerable<StockAggregate>> GetStockAggregates(IEnumerable<Guid> pharmacyMedicineIds, int expiringSoonDays = 90);

        //------------
        Task<IEnumerable<Batch>> GetBatchesForExpiryNotifications();

        Task<int> GetAvailableQuantity(Guid pharmacyMedicineId);

    }
}