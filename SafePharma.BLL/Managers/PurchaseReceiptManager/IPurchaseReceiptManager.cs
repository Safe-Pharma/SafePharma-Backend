
using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IPurchaseReceiptManager
    {
        Task<GeneralResult<ReadPurchaseReceiptDto?>> CreatePurchaseReceipt(CreatePurchaseReceiptDto createDto, Guid userId, Guid purchaseOrderId);
        Task<GeneralResult<IEnumerable<ReadPurchaseReceiptDto>>> GetAllReceipts(Guid pharmacyId);
        Task<GeneralResult<ReadPurchaseReceiptDto?>> GetReceiptById(Guid pharmacyId, Guid id);
    }
}