namespace SafePharma.BLL
{
    public class CustomerStatsDto
    {
        public int TotalCustomers { get; set; }
        public int Active { get; set; }
        public int Inactive { get; set; }
        public decimal TotalPaidAllCustomers { get; set; }
    }
}