namespace SafePharma.DAL
{
    public class SaleRepository : GenircRepository<Sale>, ISaleRepository
    {
        public SaleRepository(AppDbContext db) : base(db)
        {
        }
    }
}
