namespace SafePharma.BLL
{
    public class TaxDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public string Status { get; set; } = string.Empty; // "Active" | "Inactive"
    }
}
