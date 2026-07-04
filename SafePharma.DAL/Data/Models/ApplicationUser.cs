using Microsoft.AspNetCore.Identity;

namespace SafePharma.DAL;

public class ApplicationUser : IdentityUser<Guid> , IAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public string? Branch { get; set; } = "Main Branch";
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //public string CreatedBy { get; set; }=string.Empty;

    public DateTime? UpdatedAt { get; set; }
    //public string? UpdatedBy { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; } = string.Empty;


    //tenant should be done (required)
    public Guid? PharmacyId { get; set; }
    public Pharmacy? Pharmacy { get; set; } = null!;
    public virtual ICollection<Audit> AuditList { get; set; } = new HashSet<Audit>();
    public string PreferredLanguage { get; set; } = "en";
    public string FullName => $"{FirstName} {LastName}".Trim();
}