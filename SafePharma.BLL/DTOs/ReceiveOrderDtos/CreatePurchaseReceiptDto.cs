namespace SafePharma.BLL
{
    public class CreatePurchaseReceiptDto
    {
        public string? InvoiceNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public decimal? InvoiceTotal { get; set; }

        public List<CreatePurchaseReceiptItemDto> Items { get; set; }
            = new();
    }
}
