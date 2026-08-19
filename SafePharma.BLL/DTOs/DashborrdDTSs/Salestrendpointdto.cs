namespace SafePharma.BLL
{
    public class SalesTrendPointDto
    {
        public DateTime Date { get; set; }
        public string DayLabel { get; set; } = string.Empty; // e.g. "Mon"
        public decimal Total { get; set; }
        public int OrderCount { get; set; }
    }
}