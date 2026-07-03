using Microsoft.AspNetCore.Identity;

namespace SafePharma.DAL
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
        
    }
}
