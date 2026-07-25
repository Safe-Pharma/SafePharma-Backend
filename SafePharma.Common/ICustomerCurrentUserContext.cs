public interface ICustomerCurrentUserContext
{
    Guid CustomerId { get; }
    string Phone { get; }
    string Name { get; }
    bool IsAuthenticated { get; }
}