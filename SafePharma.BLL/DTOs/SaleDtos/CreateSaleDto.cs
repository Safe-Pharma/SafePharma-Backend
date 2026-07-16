using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class CreateSaleDto
    {
        public Guid PharmacyId { get; set; }
        public Guid ApplicationUserId { get; set; }
        public Guid? CustomerId { get; set; }
        public SalePaymentMethod PaymentMethod { get; set; } = SalePaymentMethod.Cash;
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal  GrandTotal { get; set; }
        public decimal SubTotal { get; set; }

        public decimal AmountPaidByCash { get; set; }
        public decimal AmountPaidByCard { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Change { get; set; }
        public SaleStatus Status { get; set; } = SaleStatus.Open;
        public List<CreateSaleItemsDto> Items { get; set; } = new();

    }
}
