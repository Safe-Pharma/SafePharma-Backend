using SafePharma.DAL.Data.Models;

namespace SafePharma.DAL
{
    public enum CustomerStatus
    {
        Active,
        Inactive
    }

    public class Customer : IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Notes { get; set; }
        public CustomerStatus Status { get; set; } = CustomerStatus.Active;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<CustomerAllergy> CustomerAllergies { get; set; } = new HashSet<CustomerAllergy>();
        public ICollection<CustomerChronicCondition> CustomerChronicConditions { get; set; } = new HashSet<CustomerChronicCondition>();
        public virtual ICollection<CustomerMedicineHistory> MedicineHistory { get; set; } = new HashSet<CustomerMedicineHistory>();
        public virtual ICollection<CustomerPharmacyBalance> PharmacyBalances { get; set; } = new HashSet<CustomerPharmacyBalance>();
        public ICollection<CustomerOrganFunction> CustomerOrganFunctions { get; set; } = new HashSet<CustomerOrganFunction>();
        public ICollection<CustomerRelative> Relatives { get; set; }
             = new HashSet<CustomerRelative>();

        public ICollection<CustomerRelative> RelatedTo { get; set; }
            = new HashSet<CustomerRelative>();
    }
}