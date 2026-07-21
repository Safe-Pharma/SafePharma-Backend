using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class BatchManager : IBatchManager
    {
        public IUnitOfWork _unitOfWork;
        public IAuditManager _auditManager;

        public BatchManager(IUnitOfWork unitOfWork, IAuditManager auditManager = null)
        {
            _unitOfWork = unitOfWork;
            _auditManager = auditManager;
        }

        public async Task<GeneralResult<IEnumerable<BatchReadDto>>> GetAllBatches()
        {

            var batchList = await _unitOfWork._batchRepository.GetBatchesGroupByhMedicine();
            if(batchList is null)
            {
                return GeneralResult<IEnumerable<BatchReadDto>>.NotFound("No Batches Founded");
            }

            IEnumerable<BatchReadDto> batchReadList = batchList.Select(group =>
            {
                var Med = group.First();

                int minStock = Med.Medicine.MinStockLevel;
                int stock = group.Sum(b => b.QuantityRemaining);

                return new BatchReadDto
                {
                    BatchesCount = group.Count(),
                    MedeicineCategory = Med.Medicine.Medicine.Category,
                    MedeicineName = Med.Medicine.Medicine.TradeNameEn,
                    SKU = Med.Medicine.SKU,
                    MinStockLevel = minStock,
                    OnHand = stock,
                    Batches = group.Select(b => new BatchItemDto

                    {
                        Id = b.Id,
                        BatchNumber = b.BatchNumber,
                        ExpiryDate = b.ExpiryDate,
                        QuantityRemaining = b.QuantityRemaining,
                        DaysLeft = (b.ExpiryDate - DateTime.Now).Days >= 0 ? (b.ExpiryDate - DateTime.Now).Days : 0,
                    }).ToList(),
                    StockLevel = (
                                    stock == 0
                                        ? StockLevelEnum.Out
                                        : stock <= minStock
                                            ? StockLevelEnum.Low
                                            : StockLevelEnum.InStock
                                ).ToString()
                };
            }).ToList();
            return GeneralResult<IEnumerable<BatchReadDto>>.SuccessResult(batchReadList);
        }

        public async Task<GeneralResult<Batch>> CreateBatch(BatchCreateDto batchDto)
        {
            if (batchDto is null)
            {
                GeneralResult<BatchCreateDto>.NotFound();
            }
            // get medecine form medicine manager
            var pharmacyMedicine = await _unitOfWork.PharmacyMedicineRepository.GetById(batchDto!.MedicineId);
            var recieptItem = await _unitOfWork.PurchaseReceiptItemRepository.GetById(batchDto!.ReceiptItemId);
            var batch = new Batch
            {
                MedicineId = recieptItem.PharmacyMedicineId,
                Medicine = pharmacyMedicine,

                PurchaseReceiptItemId = batchDto.ReceiptItemId,
                PurchaseReceiptItem = recieptItem,

                BatchNumber = recieptItem.BatchNumber,
                ExpiryDate = recieptItem.ExpiryDate,
                QuantityReceived = recieptItem.Quantity,
                QuantityRemaining = recieptItem.Quantity,

                SellingPrice = pharmacyMedicine.SellingPrice,
                PurchasePrice = pharmacyMedicine.PurchasePrice,



            };
            _unitOfWork._batchRepository.Add(batch);
            await _unitOfWork.SaveAsync();

            return GeneralResult<Batch>.SuccessResult(batch);
        }

        public async Task<GeneralResult> DeleteBatch(Guid id)
        {

            var batch = await _unitOfWork._batchRepository.GetById(id);
            if (batch is null)
            {
                return GeneralResult.NotFound("Batch not found");
            }
            _unitOfWork._batchRepository.Delete(batch);
            await _unitOfWork.SaveAsync();


            return GeneralResult.SuccessResult();
        }
        public async Task<GeneralResult> UpdateBatchQuantitiy(Guid id, int newStock)
        {

            var batch = await _unitOfWork._batchRepository.GetById(id);
            if (batch is null)
            {
                return GeneralResult.NotFound("Batch not found");
            }

            var tempObj = new Batch
            {
                Id = batch.Id,
                MedicineId = batch.MedicineId,
                PurchaseReceiptItemId = batch.PurchaseReceiptItemId,
                BatchNumber = batch.BatchNumber,
                ExpiryDate = batch.ExpiryDate,
                QuantityReceived = batch.QuantityReceived,
                QuantityRemaining = batch.QuantityRemaining,
                SellingPrice = batch.SellingPrice,
                PurchasePrice = batch.PurchasePrice,
                CreatedAt = batch.CreatedAt,
                UpdatedAt = batch.UpdatedAt
            };

            batch.QuantityRemaining = newStock;
            await _unitOfWork.SaveAsync();
            await _auditManager.CreateAudit(batch, tempObj, ActionsEnum.Update);


            return GeneralResult.SuccessResult();
        }


    }
}
