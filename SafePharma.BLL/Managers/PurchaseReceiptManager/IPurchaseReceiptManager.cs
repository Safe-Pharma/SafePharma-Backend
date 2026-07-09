
using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IPurchaseReceiptManager
    {
        Task<GeneralResult<ReadPurchaseReceiptDto?>> CreatePurchaseReceipt(CreatePurchaseReceiptDto createDto, Guid userId, Guid purchaseOrderId);    }
}