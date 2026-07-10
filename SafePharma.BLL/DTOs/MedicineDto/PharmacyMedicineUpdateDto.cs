namespace SafePharma.BLL
{
    public class PharmacyMedicineUpdateDto
    {
        public List<Guid> TaxIds { get; set; } = new();
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int MinStockLevel { get; set; }
        public string SKU { get; set; } = string.Empty;
    }
}