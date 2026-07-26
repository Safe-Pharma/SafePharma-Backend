namespace SafePharma.BLL
{
    public class CustomerUpdatePortalDto
    {
        required
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Notes { get; set; }
    }
}