using SafePharma.Common;
using SafePharma.DAL;
using System.Text.Json;

namespace SafePharma.BLL
{
    public class BatchManager : IBatchManager
    {
        public IUnitOfWork _unitOfWork;

        public BatchManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResult<IEnumerable<AuditReadDto>>> GetAllBatches()
        {

            var auditList = await _unitOfWork._auditRepository.GetAuditsWithUsers();
            IEnumerable<AuditReadDto> auditReadList = auditList.Select(a => new AuditReadDto
            {
                Entity = a.Entity,
                Action = a.Action,
                Date = a.Date,
                Device = a.Device,
                UserFullName = a.User.UserName!,
                oldValues = string.IsNullOrWhiteSpace(a.oldValues)
                            ? null
                            : JsonSerializer.Deserialize<JsonElement>(a.oldValues),
                newValues = string.IsNullOrWhiteSpace(a.newValues)
                            ? null
                            : JsonSerializer.Deserialize<JsonElement>(a.newValues)
            }).ToList();
            return GeneralResult<IEnumerable<AuditReadDto>>.SuccessResult(auditReadList);
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

    }
}
