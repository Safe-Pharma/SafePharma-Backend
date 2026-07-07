
namespace SafePharma.DAL
{
    public interface IPurchaseOrderRepository : IGenircRepository<PurchaseOrder>
    {
        Task<IEnumerable<PurchaseOrder>> GetAllWithSupplierAsync(Guid pharmacyId);
        Task<PurchaseOrder?> GetByIdWithDetailsAsync(Guid id);
    }
}