namespace SafePharma.DAL
{
    public interface IPaymentVerificationRepository : IGenircRepository<PaymentVerification>
    {
        Task<PaymentVerification?> GetByIdWithSubscription(Guid id);
        Task<IEnumerable<PaymentVerification>> GetPendingWithSubscription();
        Task<bool> HasPendingForSubscription(Guid subscriptionId);
        Task<PaymentVerification?> GetLatestForSubscription(Guid subscriptionId);
        Task<IEnumerable<PaymentVerification>> GetAllWithSubscription();
    }
}