namespace SafePharma.BLL
{
    public class CreateSaleDto
    {
        public Guid PharmacyId { get; set; }
        public Guid ApplicationUserId { get; set; }
        public Guid? CustomerId { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; } = "Open";
        public List<CreateSaleItemsDto> Items { get; set; } = new();

    }
}
