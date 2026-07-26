using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface ICustomerManager
    {
        // Customer list/detail/stats are viewed from within a pharmacy, and TotalPaid on
        // CustomerDto reflects payments AT THAT PHARMACY ONLY (see CustomerPharmacyBalance).
        Task<IEnumerable<CustomerDto>> GetAllCustomers(Guid pharmacyId, string? search = null);
        Task<CustomerDto?> GetCustomerById(Guid pharmacyId, Guid id);
        Task<CustomerStatsDto> GetStats(Guid pharmacyId);
        Task<CustomerCreateResult> CreateCustomer(CustomerCreateDto dto);
        Task<CustomerUpdateResult> UpdateCustomer(Guid id, CustomerUpdateDto dto);



        //portal
        Task<GeneralResult<CustomerDto?>> GetMe(Guid id);
        Task<GeneralResult<CustomerUpdateResult>> UpdateCustomerPortal(
               Guid id,
               CustomerUpdatePortalDto dto);







        Task<bool> DeleteCustomer(Guid id);
        Task<CustomerDto?> ToggleStatus(Guid pharmacyId, Guid id);

        // Records a payment from this customer at this pharmacy — creates the
        // per-pharmacy balance row on first payment, otherwise adds to it.
        Task<RecordCustomerPaymentResult> RecordPayment(Guid pharmacyId, Guid customerId, decimal amount);

        // Medicine history: what this customer bought, and what they are currently taking.
        Task<IEnumerable<CustomerMedicineHistoryDto>?> GetMedicineHistory(Guid customerId, bool? isActive = null);
        Task<AddCustomerMedicineHistoryResult> AddMedicineHistory(Guid customerId, CreateCustomerMedicineHistoryDto dto);
        Task<CustomerMedicineHistoryDto?> ToggleMedicineActive(Guid customerId, Guid historyId);
        Task<bool> DeleteMedicineHistory(Guid customerId, Guid historyId);


        // --- Allergies ---
        Task<IEnumerable<CustomerAllergyDto>?> GetAllergies(Guid customerId);
        Task<AssignResult> AssignAllergy(Guid customerId, Guid allergyId);
        Task<bool> RemoveAllergy(Guid customerId, Guid allergyId);

        // --- Chronic conditions ---
        Task<IEnumerable<CustomerChronicConditionDto>?> GetChronicConditions(Guid customerId);
        Task<AssignResult> AssignChronicCondition(Guid customerId, Guid chronicConditionId);
        Task<bool> RemoveChronicCondition(Guid customerId, Guid chronicConditionId);

        // --- Organ functions (organ + impairment level, one record per organ per customer) ---
        Task<IEnumerable<CustomerOrganFunctionDto>?> GetOrganFunctions(Guid customerId);
        Task<AssignOrganFunctionResult> AssignOrganFunction(Guid customerId, AssignOrganFunctionDto dto);
        Task<bool> RemoveOrganFunction(Guid customerId, Guid organFunctionId);
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

    public class RecordCustomerPaymentResult
    {
        public CustomerDto? Customer { get; set; }
        public bool CustomerNotFound { get; set; }
    }

    public class AddCustomerMedicineHistoryResult
    {
        public CustomerMedicineHistoryDto? History { get; set; }
        public bool CustomerNotFound { get; set; }
        public bool MedicineNotFound { get; set; }
        public bool WasUpdated { get; set; }
    }

    // Shared by Allergy and ChronicCondition assignment — same three outcomes either way.
    public class AssignResult
    {
        public bool CustomerNotFound { get; set; }
        public bool ReferenceNotFound { get; set; } // the Allergy id doesn't exist
        public bool AlreadyAssigned { get; set; }
    }

    public class AssignOrganFunctionResult
    {
        public CustomerOrganFunctionDto? OrganFunction { get; set; }
        public bool CustomerNotFound { get; set; }
        public bool OrganNotFound { get; set; }
        public bool ImpairmentLevelNotFound { get; set; }
    }
}