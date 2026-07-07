using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IPurchaseOrderManager
    {
        Task<GeneralResult<PurchaseOrderReadDto>> CreateAsync(PurchaseOrderCreateDto createDto, Guid pharmacyId);
        Task<GeneralResult<IEnumerable<PurchaseOrderReadDto>>> GetAllAsync(Guid pharmacyId);
    }
}