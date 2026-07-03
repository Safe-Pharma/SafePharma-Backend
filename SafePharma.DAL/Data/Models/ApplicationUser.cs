using Microsoft.AspNetCore.Identity;

namespace SafePharma.DAL;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public virtual ICollection<Audit> AuditList { get; set; } = new HashSet<Audit>();

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }=string.Empty;

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; } = string.Empty;
}