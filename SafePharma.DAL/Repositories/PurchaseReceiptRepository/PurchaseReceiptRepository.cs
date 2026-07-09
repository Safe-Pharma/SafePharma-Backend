namespace SafePharma.DAL
{
    public class PurchaseReceiptRepository : GenircRepository<PurchaseReceipt>, IPurchaseReceiptRepository
    {
        public PurchaseReceiptRepository(AppDbContext db) : base(db)
        {
        }
    }
}
