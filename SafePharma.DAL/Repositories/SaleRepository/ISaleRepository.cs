namespace SafePharma.DAL
{
    public interface ISaleRepository : IGenircRepository<Sale>
    {
        Task<Sale?> GetByIdWithItemsAsync(Guid saleId);
        Task<IEnumerable<Sale>> GetAllForPharmacy(Guid pharmacyId, SaleStatus? status = null, string? search = null);

        // stats
        Task<decimal> GetTodayTotal(Guid pharmacyId);
        Task<int> GetCompletedCount(Guid pharmacyId);
        Task<int> GetCancelledCount(Guid pharmacyId);
        Task<decimal> GetAverageBasket(Guid pharmacyId);
    }
}