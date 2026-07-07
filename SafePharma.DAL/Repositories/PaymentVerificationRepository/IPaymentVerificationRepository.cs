namespace SafePharma.DAL
{
    public interface IPaymentVerificationRepository : IGenircRepository<PaymentVerification>
    {
        Task<PaymentVerification?> GetByIdWithSubscription(Guid id);
        Task<IEnumerable<PaymentVerification>> GetPendingWithSubscription();
        Task<bool> HasPendingForSubscription(Guid subscriptionId);
    }
}