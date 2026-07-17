// CustomerUpdateDto.cs
namespace SafePharma.BLL
{
    public class CustomerUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Active"; // "Active" | "Inactive"
    }
}