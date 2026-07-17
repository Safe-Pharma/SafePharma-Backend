using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class PaySaleDto
    {
        public SalePaymentMethod PaymentMethod { get; set; } = SalePaymentMethod.Cash;
        public decimal AmountPaid { get; set; } = 0;
        public decimal AmountPaidByCash { get; set; }
        public decimal AmountPaidByCard { get; set; }
        public decimal Change { get; set; } = 0;
        public SaleStatus Status { get; set; } = SaleStatus.Open;
    }
}
