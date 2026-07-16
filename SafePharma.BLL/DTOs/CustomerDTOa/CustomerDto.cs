namespace SafePharma.BLL
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "Active" | "Inactive"
        //public decimal Outstanding { get; set; }
    }
}