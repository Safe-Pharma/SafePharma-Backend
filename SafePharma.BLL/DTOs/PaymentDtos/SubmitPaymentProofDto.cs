using Microsoft.AspNetCore.Http;

namespace SafePharma.BLL
{
    public class SubmitPaymentProofDto
    {
        public string PaymentMethod { get; set; }
        public string TransactionReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaidAmount { get; set; }
        public IFormFile Receipt { get; set; }
    }
}