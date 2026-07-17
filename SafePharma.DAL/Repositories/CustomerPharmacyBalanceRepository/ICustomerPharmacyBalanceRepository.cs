namespace SafePharma.DAL
{
    public interface ICustomerPharmacyBalanceRepository : IGenircRepository<CustomerPharmacyBalance>
    {
        Task<IEnumerable<CustomerPharmacyBalance>> GetForCustomer(Guid customerId);
        Task<IEnumerable<CustomerPharmacyBalance>> GetForPharmacy(Guid pharmacyId);
        Task<CustomerPharmacyBalance?> GetByCustomerAndPharmacy(Guid customerId, Guid pharmacyId);
    }
}