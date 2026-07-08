namespace SafePharma.DAL
{
    public interface ISupplierPaymentRepository : IGenircRepository<SupplierPayment>
    {
        Task<IEnumerable<SupplierPayment>> GetHistoryForPharmacy(Guid pharmacyId);
        Task<int> CountForPharmacy(Guid pharmacyId);
    }
}