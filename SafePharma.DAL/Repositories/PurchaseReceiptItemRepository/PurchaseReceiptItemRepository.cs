namespace SafePharma.DAL
{
    public class PurchaseReceiptItemRepository : GenircRepository<PurchaseReceiptItem>, IPurchaseReceiptItemRepository
    {
        public PurchaseReceiptItemRepository(AppDbContext db) : base(db)
        {
        }
    }
}
