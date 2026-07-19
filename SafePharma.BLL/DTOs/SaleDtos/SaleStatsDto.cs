namespace SafePharma.BLL
{
    public class SaleStatsDto
    {
        public decimal TodayTotal { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public decimal AverageBasket { get; set; }
    }
}