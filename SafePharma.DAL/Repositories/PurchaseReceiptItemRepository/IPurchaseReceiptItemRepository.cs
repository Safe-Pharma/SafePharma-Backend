namespace SafePharma.DAL
{
    public interface IPurchaseReceiptItemRepository : IGenircRepository<PurchaseReceiptItem>
    {
        Task<PurchaseReceiptItem?> GetByIdForPharmacy(Guid id, Guid pharmacyId);
    }
}