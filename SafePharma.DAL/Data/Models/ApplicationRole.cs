using Microsoft.AspNetCore.Identity;

namespace SafePharma.DAL
{
    public class ApplicationRole : IdentityRole
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        
    }
}
