namespace SafePharma.DAL
{
    public interface IPaymentMethodRepository : IGenircRepository<PaymentMethod>
    {
        Task<IEnumerable<PaymentMethod>> GetActiveOrdered();
    }
}