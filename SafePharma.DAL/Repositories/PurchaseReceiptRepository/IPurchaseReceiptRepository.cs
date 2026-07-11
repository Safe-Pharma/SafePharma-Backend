namespace SafePharma.DAL
{
    public interface IPurchaseReceiptRepository : IGenircRepository<PurchaseReceipt>
    {
        Task<IEnumerable<PurchaseReceipt>> GetAllForPharmacy(Guid pharmacyId);
        Task<PurchaseReceipt?> GetByIdForPharmacy(Guid pharmacyId, Guid id);
    }
}