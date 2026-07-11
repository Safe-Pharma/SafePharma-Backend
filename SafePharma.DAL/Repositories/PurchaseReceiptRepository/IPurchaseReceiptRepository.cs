namespace SafePharma.DAL
{
    public interface IPurchaseReceiptRepository : IGenircRepository<PurchaseReceipt>
    {
        Task<IEnumerable<PurchaseReceipt>> GetAllWithItems();
    }
}