namespace SafePharma.BLL
{
    public class TaxStatsDto
    {
        public int TotalTaxes { get; set; }
        public int Active { get; set; }
        public int Inactive { get; set; }
        public decimal AverageRate { get; set; }
    }
}
