using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class ReadSaleDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid PharmacyId { get; set; }
        public Guid ApplicationUserId { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public SalePaymentMethod PaymentMethod { get; set; } 
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal AmountPaidByCash { get; set; }
        public decimal AmountPaidByCard { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Change { get; set; }
        public SaleStatus Status { get; set; } 

        public DateTime CreatedAt { get; set; }
        public List<ReadSaleItemsDto> Items { get; set; } = new();
    }
}
