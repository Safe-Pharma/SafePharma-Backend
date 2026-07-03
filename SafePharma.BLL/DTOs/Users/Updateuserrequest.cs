using System.ComponentModel.DataAnnotations;

namespace SafePharma.BLL
{
    public class UpdateUserRequest
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        public string? Branch { get; set; }

        public bool IsActive { get; set; }
    }
}
