using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IPurchaseReceiptManager
    {
        Task<GeneralResult<IEnumerable<ReadPurchaseReceiptDto>>> GetAllPurchaseReceipts(Guid pharmacyId);
        Task<GeneralResult<ReadPurchaseReceiptDto?>> CreatePurchaseReceipt(CreatePurchaseReceiptDto createDto, Guid userId, Guid purchaseOrderId, Guid pharmacyId);
        Task<GeneralResult> UpdateReceiptItem(Guid id, UpdatePurchaseReceiptItemDto dto, Guid pharmacyId);
    }
}