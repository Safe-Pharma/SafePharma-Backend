namespace SafePharma.BLL
{
    public class SupplierCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public string Status { get; set; } = "Active"; // "Active" | "Inactive"
        public decimal Outstanding { get; set; }
    }
}
