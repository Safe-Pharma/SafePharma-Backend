namespace SafePharma.DAL
{
    public interface ISaleRepository : IGenircRepository<Sale>
    {
        Task<Sale?> GetByIdWithItemsAsync(Guid saleId);
    }
}