using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class PurchaseReceiptManager : IPurchaseReceiptManager
    {
        private readonly IPurchaseReceiptRepository _purchaseReceiptRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseReceiptItemRepository _purchaseReceiptItemRepository;
        private readonly IUnitOfWork unitOfWork;

        public PurchaseReceiptManager(IPurchaseReceiptRepository purchaseReceiptRepository, IPurchaseOrderRepository purchaseOrderRepository, IUnitOfWork unitOfWork, IPurchaseReceiptItemRepository purchaseReceiptItemRepository)
        {
            _purchaseReceiptRepository = purchaseReceiptRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            this.unitOfWork = unitOfWork;
            _purchaseReceiptItemRepository = purchaseReceiptItemRepository;
        }

        public async Task<GeneralResult<ReadPurchaseReceiptDto?>> CreatePurchaseReceipt(CreatePurchaseReceiptDto createDto, Guid userId, Guid purchaseOrderId)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetByIdWithDetailsAsync(purchaseOrderId);

            if (purchaseOrder == null)
            {
                return GeneralResult<ReadPurchaseReceiptDto?>.NotFound("Purchase order not found.");
            }

            if (Guid.Empty == userId)
            {
                return GeneralResult<ReadPurchaseReceiptDto?>.FailResult("User ID is required.");
            }

            var receiptId = Guid.NewGuid();

            PurchaseReceipt receipt = new PurchaseReceipt()
            {
                Id = receiptId,
                PurchaseOrderId = purchaseOrderId,
                ReceivedBy = userId,
                ReceivedAt = DateTime.UtcNow,
                InvoiceNumber = createDto.InvoiceNumber,
                InvoiceDate = createDto.InvoiceDate,
                InvoiceTotal = createDto.InvoiceTotal,
                Items = purchaseOrder.Items.Select(i =>
                {
                    var dtoItem = createDto.Items
                        .FirstOrDefault(x => x.PurchaseOrderItemId == i.Id);

                    if (dtoItem == null)
                        return null;

                    return new PurchaseReceiptItem
                    {
                        Id = Guid.NewGuid(),
                        PurchaseReceiptId = receiptId,
                        PurchaseOrderItemId = i.Id,
                        MedicineId = i.MedicineId,
                        MedicineName = i.Medicine.TradeNameEn,
                        Quantity = i.QuantityOrdered,
                        UnitPrice = i.UnitPrice,
                        BatchNumber = dtoItem.BatchNumber,
                        ExpiryDate = dtoItem.ExpiryDate
                    };
                })
                        .Where(x => x != null)
                        .ToList()!
            };
            _purchaseReceiptRepository.Add(receipt);
            purchaseOrder.Status = "Received";

            await unitOfWork.SaveAsync();

            var dtoResult = new ReadPurchaseReceiptDto
            {
                PurchaseOrderId = receipt.PurchaseOrderId,
                InvoiceNumber = receipt.InvoiceNumber,
                InvoiceDate = receipt.InvoiceDate,
                InvoiceTotal = receipt.InvoiceTotal,
                ReceivedBy = receipt.ReceivedBy,
                ReceivedAt = receipt.ReceivedAt,
            };
            return GeneralResult<ReadPurchaseReceiptDto?>.SuccessResult(dtoResult);
        }
    }
}
