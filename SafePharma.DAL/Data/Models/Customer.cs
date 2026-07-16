using SafePharma.DAL;

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

    public decimal TotalPaid { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CustomerMedicineHistory> MedicineHistory { get; set; } = new HashSet<CustomerMedicineHistory>();
}