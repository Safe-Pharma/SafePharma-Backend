
namespace SafePharma.DAL
{
    public interface IPurchaseOrderRepository
    {
        Task<IEnumerable<PurchaseOrder>> GetAllWithSupplierAsync();
        Task<PurchaseOrder?> GetByIdWithDetailsAsync(Guid id);
    }
}