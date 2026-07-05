using System.ComponentModel.DataAnnotations;

namespace SafePharma.BLL
{
    public class CreateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string? Branch { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
