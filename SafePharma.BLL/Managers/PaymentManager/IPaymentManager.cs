using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IPaymentManager
    {
        Task<GeneralResult<PaymentInstructionsDto>> GetPaymentInstructions(Guid subscriptionId);
        Task<GeneralResult<PaymentVerificationReadDto>> SubmitPaymentProof(Guid subscriptionId, SubmitPaymentProofDto dto);
        Task<IEnumerable<PaymentVerificationReadDto>> GetPendingVerifications();
        Task<GeneralResult> ApprovePayment(Guid verificationId, Guid reviewedByUserId);
        Task<GeneralResult> RejectPayment(Guid verificationId, Guid reviewedByUserId, string reason);
        Task<GeneralResult<PaymentVerificationReadDto>> GetLatestVerificationStatus(Guid subscriptionId);
        Task<IEnumerable<PaymentVerificationReadDto>> GetAllVerifications();
    }
}