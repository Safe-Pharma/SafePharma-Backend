using FluentValidation;
using Microsoft.AspNetCore.Http;
using SafePharma.BLL.DTOs;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class BatchManager : IBatchManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditManager _auditManager;
        private readonly IValidator<BatchQtyDto> _updateBatchQtyValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserContext _currentUserContext;

        public BatchManager(IUnitOfWork unitOfWork, IAuditManager auditManager, IValidator<BatchQtyDto> updateBatchQtyValidator, ICurrentUserContext currentUserContext, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _auditManager = auditManager;
            _updateBatchQtyValidator = updateBatchQtyValidator;
            _currentUserContext = currentUserContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GeneralResult<IEnumerable<BatchReadDto>>> GetAllBatches()
        {
            var pharmacyId = _currentUserContext.PharmacyId;

            var batchList = await _unitOfWork._batchRepository.GetBatchesGroupByMedicineAsync(pharmacyId);
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
                    MedeicineCategory = Med.Medicine.Medicine!.Category,
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

        public async Task<GeneralResult<Batch>> CreateBatch(
            BatchCreateDto batchDto)
        {
            if (batchDto is null)
            {
                return GeneralResult<Batch>.NotFound(
                    "Batch data is required");
            }

            var pharmacyId = _currentUserContext.PharmacyId;

            var pharmacyMedicine =
                await _unitOfWork.PharmacyMedicineRepository
                    .GetByMedicineAndPharmacy(
                        batchDto.MedicineId,
                        pharmacyId);

            if (pharmacyMedicine is null)
            {
                return GeneralResult<Batch>.NotFound(
                    $"Medicine with ID {batchDto.MedicineId} not found");
            }

            var receiptItem =
                await _unitOfWork.PurchaseReceiptItemRepository
                    .GetByIdForPharmacy(
                        batchDto.ReceiptItemId,
                        pharmacyId);

            if (receiptItem is null)
            {
                return GeneralResult<Batch>.NotFound(
                    $"Receipt item with ID {batchDto.ReceiptItemId} not found");
            }

             if (receiptItem.PharmacyMedicineId != pharmacyMedicine.Id)
            {
                return GeneralResult<Batch>.FailResult(
                    "The receipt item does not belong to the selected medicine.");
            }

            var batch = new Batch
            {
                MedicineId = pharmacyMedicine.Id,

                PurchaseReceiptItemId = receiptItem.Id,

                PharmacyId = pharmacyId,

                BatchNumber = receiptItem.BatchNumber,

                ExpiryDate = receiptItem.ExpiryDate,

                QuantityReceived = receiptItem.Quantity,

                QuantityRemaining = receiptItem.Quantity,

                SellingPrice = pharmacyMedicine.SellingPrice,

                PurchasePrice = pharmacyMedicine.PurchasePrice,

                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork._batchRepository.Add(batch);

            await _unitOfWork.SaveAsync();

            return GeneralResult<Batch>.SuccessResult(batch);
        }
        public async Task<GeneralResult> DeleteBatch(Guid id)
        {
            var batch = await _unitOfWork._batchRepository
                .GetByIdForPharmacyAsync(id, _currentUserContext.PharmacyId);

            if (batch is null)
                return GeneralResult.NotFound("Batch not found");

            
                batch.IsDeleted = true;
                batch.DeletedAt = DateTime.UtcNow;
                batch.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveAsync();
            return GeneralResult.SuccessResult("Batch deleted successfully");
        }
        public async Task<GeneralResult> UpdateBatchQuantity(BatchQtyDto dto)
        {
            if (dto is null)
                return GeneralResult.NotFound("Batch data is required");

            var pharmacyId = _currentUserContext.PharmacyId;

            var batch = await _unitOfWork._batchRepository
                .GetByIdForPharmacyAsync(
                    dto.BatchId,
                    pharmacyId);

            if (batch is null)
                return GeneralResult.NotFound("Batch not found");

            var validationResult =
                await _updateBatchQtyValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new Error
                        {
                            ErrorCode = e.ErrorCode,
                            ErrorMessage = e.ErrorMessage
                        }).ToList()
                    );

                return GeneralResult<TokenDto>.FailResult(
                    errors,
                    "Validation failed");
            }

            var oldValues = new
            {
                batch.QuantityRemaining,
                batch.UpdatedAt
            };

            batch.QuantityRemaining = dto.NewStock;
            batch.UpdatedAt = DateTime.UtcNow;


            var newValues = new
            {
                batch.QuantityRemaining,
                batch.UpdatedAt
            };

            await _auditManager.CreateAudit(
                newValues,
                oldValues,
                "Batch",
                ActionsEnum.Update);

            await _unitOfWork.SaveAsync();

            return GeneralResult.SuccessResult();
        }

    }
}
