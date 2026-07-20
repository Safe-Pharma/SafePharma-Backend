namespace SafePharma.DAL
{
    public interface ICustomerMedicineHistoryRepository : IGenircRepository<CustomerMedicineHistory>
    {
        Task<IEnumerable<CustomerMedicineHistory>> GetForCustomer(Guid customerId, bool? isActive = null);
        Task<CustomerMedicineHistory?> GetByIdForCustomer(Guid id, Guid customerId);
        Task<CustomerMedicineHistory?> GetByCustomerAndMedicine(Guid customerId, Guid? medicineId);
    }
}
