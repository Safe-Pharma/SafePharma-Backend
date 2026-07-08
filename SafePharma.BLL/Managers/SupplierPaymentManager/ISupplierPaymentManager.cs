namespace SafePharma.BLL
{
    public interface ISupplierPaymentManager
    {
        Task<IEnumerable<SupplierPaymentDto>> GetHistory(Guid pharmacyId);
        Task<RecordPaymentResult> RecordPayment(Guid pharmacyId, Guid recordedByUserId, RecordSupplierPaymentDto dto);
    }

    public class RecordPaymentResult
    {
        public SupplierPaymentDto? Payment { get; set; }
        public bool SupplierNotFound { get; set; }
        public bool AmountExceedsBalance { get; set; }
    }
}
