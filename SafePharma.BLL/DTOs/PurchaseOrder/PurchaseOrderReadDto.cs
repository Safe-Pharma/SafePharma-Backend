using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class PurchaseOrderReadDto
    {
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string SupplierName { get; set; } = null!;
        public int Lines { get; set; }

        }
}
