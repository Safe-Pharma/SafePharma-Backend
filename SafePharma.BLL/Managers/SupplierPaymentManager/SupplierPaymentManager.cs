using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class SupplierPaymentManager : ISupplierPaymentManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierPaymentManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SupplierPaymentDto>> GetHistory(Guid pharmacyId)
        {
            var payments = await _unitOfWork.SupplierPaymentRepository.GetHistoryForPharmacy(pharmacyId);
            return payments.Select(p => p.ToDto());
        }

        public async Task<RecordPaymentResult> RecordPayment(
            Guid pharmacyId, Guid recordedByUserId, RecordSupplierPaymentDto dto)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetById(dto.SupplierId);

            if (supplier is null || supplier.PharmacyId != pharmacyId)
            {
                return new RecordPaymentResult { SupplierNotFound = true };
            }

            if (dto.Amount > supplier.Outstanding)
            {
                return new RecordPaymentResult { AmountExceedsBalance = true };
            }

            var payment = new SupplierPayment
            {
                Id = Guid.NewGuid(),
                SupplierId = supplier.Id,
                RecordedBy = recordedByUserId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Note = dto.Note,
                PaidAt = dto.PaidAt,
                CreatedAt = DateTime.UtcNow,
            };

            supplier.Outstanding -= dto.Amount;
            supplier.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.SupplierPaymentRepository.Add(payment);
            await _unitOfWork.SaveAsync();

            // reuse the already-loaded supplier instead of re-querying for the DTO mapping
            payment.Supplier = supplier;

            return new RecordPaymentResult { Payment = payment.ToDto() };
        }
    }
}
