namespace SafePharma.DAL
{
    public interface ISaleRepository : IGenircRepository<Sale>
    {
        Task<Sale?> GetByIdWithItemsAsync(Guid saleId);
        Task<IEnumerable<Sale>> GetAllForPharmacy(Guid pharmacyId, SaleStatus? status = null);
    }
}