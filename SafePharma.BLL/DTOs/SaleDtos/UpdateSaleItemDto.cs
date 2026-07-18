namespace SafePharma.BLL
{
    public class UpdateSaleItemDto
    {
        public Guid CustomerId { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
    }
}
