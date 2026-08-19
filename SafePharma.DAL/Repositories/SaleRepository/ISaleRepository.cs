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

        // dashboard
        Task<IEnumerable<(DateTime Date, decimal Total, int OrderCount)>> GetDailyTotals(Guid pharmacyId, int days);
        Task<IEnumerable<(string Category, decimal Revenue)>> GetCategoryRevenue(Guid pharmacyId);


        Task<IEnumerable<Sale>> GetByCustomerIdAsync(
           Guid customerId,
           string? search = null,
           Guid? pharmacyId = null,
           SaleStatus? status = null,
           DateTime? from = null,
           DateTime? to = null,
           int page = 1,
           int pageSize = 10);
        Task<Sale?> GetByIdWithItemsAndCustomerIdAsync(Guid saleId,Guid customerId);



    }
}