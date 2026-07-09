namespace SafePharma.BLL
{
    public class CreatePurchaseReceiptItemDto
    {
        public Guid PurchaseOrderItemId { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
