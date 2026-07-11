
using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IPurchaseReceiptManager
    {
        Task<GeneralResult<IEnumerable<ReadPurchaseReceiptDto>>> GetAllPurchaseReceipts();
        Task<GeneralResult<ReadPurchaseReceiptDto?>> CreatePurchaseReceipt(CreatePurchaseReceiptDto createDto, Guid userId, Guid purchaseOrderId);
        Task<GeneralResult> UpdateReceiptItem(Guid id, UpdatePurchaseReceiptItemDto dto);
    }
}