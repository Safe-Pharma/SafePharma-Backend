namespace SafePharma.BLL
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty; // Country.Name
        public string Status { get; set; } = string.Empty;  // "Active" | "Inactive"
        public decimal Outstanding { get; set; }
    }
}
