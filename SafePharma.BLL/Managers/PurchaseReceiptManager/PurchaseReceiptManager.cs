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

        public async Task<GeneralResult<IEnumerable<ReadPurchaseReceiptDto>>> GetAllPurchaseReceipts()
        {
            var receipts = await _purchaseReceiptRepository.GetAllWithItems();

            var receiptDtos = receipts.Select(r => new ReadPurchaseReceiptDto
            {

                PurchaseOrderId = r.PurchaseOrderId,
                InvoiceNumber = r.InvoiceNumber,
                InvoiceDate = r.InvoiceDate,
                InvoiceTotal = r.InvoiceTotal,
                ReceivedBy = r.ReceivedBy,
                ReceivedAt = r.ReceivedAt,

                Items = r.Items.Select(i => new ReadPurchaseReceiptItemDto
                {
                    PurchaseReceiptItemId = i.Id,
                    PurchaseOrderItemId = i.PurchaseOrderItemId,
                    BatchNumber = i.BatchNumber,
                    ExpiryDate = i.ExpiryDate,
                    MedicineName = i.PurchaseOrderItem.PharmacyMedicine.TradeNameEn,
                    Quantity = i.PurchaseOrderItem.QuantityOrdered,
                    UnitPrice = i.UnitPrice,
                    SellingPrice = i.SellingPrice,
                    PharmacyMedicineId = i.PurchaseOrderItem.PharmacyMedicineId
                }).ToList()
            });

            return GeneralResult<IEnumerable<ReadPurchaseReceiptDto>>
                .SuccessResult(receiptDtos);
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


            var supplier = await unitOfWork.SupplierRepository.GetById(purchaseOrder.SupplierId);
            if (supplier is null)
            {
                return GeneralResult<ReadPurchaseReceiptDto?>.NotFound("Supplier not found.");
            }

            var receiptId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var receiptItems = purchaseOrder.Items
                .Select(i =>
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
                        PharmacyMedicineId = i.PharmacyMedicineId,
                        MedicineName = i.PharmacyMedicine.TradeNameEn,
                        Quantity = i.QuantityOrdered,
                        UnitPrice = i.UnitPrice,
                        SellingPrice = i.SellingPrice,
                        BatchNumber = dtoItem.BatchNumber,
                        ExpiryDate = dtoItem.ExpiryDate
                    };
                })
                .Where(x => x != null)
                .ToList()!;

            PurchaseReceipt receipt = new PurchaseReceipt()
            {
                Id = receiptId,
                PurchaseOrderId = purchaseOrderId,
                ReceivedBy = userId,
                ReceivedAt = now,
                InvoiceNumber = createDto.InvoiceNumber,
                InvoiceDate = createDto.InvoiceDate,
                InvoiceTotal = createDto.InvoiceTotal,
                Items = receiptItems
            };

            _purchaseReceiptRepository.Add(receipt);
            purchaseOrder.Status = "Received";

            foreach (var item in receiptItems)
            {
                unitOfWork._batchRepository.Add(new Batch
                {
                    Id = Guid.NewGuid(),
                    MedicineId = item.PharmacyMedicineId,
                    PurchaseReceiptItemId = item.Id,
                    BatchNumber = item.BatchNumber,
                    ExpiryDate = item.ExpiryDate,
                    QuantityReceived = item.Quantity,
                    QuantityRemaining = item.Quantity,
                    PurchasePrice = item.UnitPrice,
                    SellingPrice = item.SellingPrice,
                    CreatedAt = now,
                });
            }

            supplier.Outstanding += (decimal)createDto.InvoiceTotal;
            supplier.UpdatedAt = now;


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

        public async Task<GeneralResult> UpdateReceiptItem(Guid id, UpdatePurchaseReceiptItemDto dto)
        {
            var item = await _purchaseReceiptItemRepository.GetById(id);

            if (item == null)
                return GeneralResult.FailResult("Receipt Item not found.");

            item.UnitPrice = dto.UnitPrice;
            item.SellingPrice = dto.SellingPrice;

            var batch = await unitOfWork._batchRepository
                .GetByPurchaseReceiptItemId(id);

            if (batch != null)
            {
                batch.PurchasePrice = dto.UnitPrice;
                batch.SellingPrice = dto.SellingPrice;
                batch.UpdatedAt = DateTime.UtcNow;
            }

            await unitOfWork.SaveAsync();

            return GeneralResult.SuccessResult();
        }
    }
}
