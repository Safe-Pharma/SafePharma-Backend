namespace SafePharma.DAL
{
    public interface IPurchaseReceiptRepository : IGenircRepository<PurchaseReceipt>
    {
        Task<IEnumerable<PurchaseReceipt>> GetAllWithItems();
        Task<IEnumerable<PurchaseReceipt>> GetAllForPharmacy(Guid pharmacyId);
        Task<PurchaseReceipt?> GetByIdWithDetailsAsync(Guid id, Guid pharmacyId);
    }
}