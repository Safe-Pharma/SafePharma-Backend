using System.ComponentModel.DataAnnotations;

namespace SafePharma.BLL
{
    public class UserDetailDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<UserActivityDto> RecentActivity { get; set; } = [];
        public string Name { get; set; }

        public Guid PharmacyId { get; set; }

        public string Email { get; set; } = string.Empty;

        
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;


        public string? Branch { get; set; }

        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
