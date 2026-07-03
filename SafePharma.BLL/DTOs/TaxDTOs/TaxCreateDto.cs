namespace SafePharma.BLL
{
    public class TaxCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public string Status { get; set; } = "Active"; // "Active" | "Inactive"
    }
}
