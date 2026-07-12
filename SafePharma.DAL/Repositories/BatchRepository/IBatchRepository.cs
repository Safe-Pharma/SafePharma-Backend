namespace SafePharma.DAL
{
    public interface IBatchRepository : IGenircRepository<Batch>
    {
        Task<IEnumerable<IGrouping<Guid, Batch>>> GetBatchesGroupByhMedicine();
        Task<IEnumerable<Batch>> GetBatchesByhMedicineId(Guid MId);
        Task<IEnumerable<StockAggregate>> GetStockAggregates(IEnumerable<Guid> pharmacyMedicineIds);
        Task<Batch?> GetByPurchaseReceiptItemId(Guid purchaseReceiptItemId);
        Task<IEnumerable<StockAggregate>> GetStockAggregates(IEnumerable<Guid> pharmacyMedicineIds, int expiringSoonDays = 90);

    }
}