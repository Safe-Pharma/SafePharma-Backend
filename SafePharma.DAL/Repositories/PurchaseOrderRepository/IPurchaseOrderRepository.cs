
namespace SafePharma.DAL
{
    public interface IPurchaseOrderRepository : IGenircRepository<PurchaseOrder>
    {
        Task<IEnumerable<PurchaseOrder>> GetAllAsync(Guid pharmacyId);
        Task<PurchaseOrder?> GetByIdWithDetailsAsync(Guid id);
    }
}