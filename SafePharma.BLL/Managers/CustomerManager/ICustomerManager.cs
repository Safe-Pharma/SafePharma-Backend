namespace SafePharma.BLL
{
    public interface ICustomerManager
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomers(string? search = null);
        Task<CustomerDto?> GetCustomerById(Guid id);
        Task<CustomerStatsDto> GetStats();
        Task<CustomerCreateResult> CreateCustomer(CustomerCreateDto dto);
        Task<CustomerUpdateResult> UpdateCustomer(Guid id, CustomerUpdateDto dto);
        Task<bool> DeleteCustomer(Guid id);
        Task<CustomerDto?> ToggleStatus(Guid id);

        // Medicine history: what this customer bought, and what they are currently taking.
        Task<IEnumerable<CustomerMedicineHistoryDto>?> GetMedicineHistory(Guid customerId, bool? isActive = null);
        Task<AddCustomerMedicineHistoryResult> AddMedicineHistory(Guid customerId, CreateCustomerMedicineHistoryDto dto);
        Task<CustomerMedicineHistoryDto?> ToggleMedicineActive(Guid customerId, Guid historyId);
        Task<bool> DeleteMedicineHistory(Guid customerId, Guid historyId);
    }

    public class CustomerCreateResult
    {
        public CustomerDto? Customer { get; set; }
        public bool DuplicatePhone { get; set; }
    }

    public class CustomerUpdateResult
    {
        public CustomerDto? Customer { get; set; }
        public bool NotFound { get; set; }
        public bool DuplicatePhone { get; set; }
    }

    public class AddCustomerMedicineHistoryResult
    {
        public CustomerMedicineHistoryDto? History { get; set; }
        public bool CustomerNotFound { get; set; }
        public bool MedicineNotFound { get; set; }
    }
}
