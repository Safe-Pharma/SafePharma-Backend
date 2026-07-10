namespace SafePharma.BLL
{
    public class PharmacyMedicineUpdateDto
    {
        public Guid TaxId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int MinStockLevel { get; set; }
        public string SKU { get; set; } = string.Empty;
    }
}