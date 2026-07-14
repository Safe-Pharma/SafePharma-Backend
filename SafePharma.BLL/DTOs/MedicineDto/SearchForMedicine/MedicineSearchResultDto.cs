namespace SafePharma.BLL
{
    public class MedicineSearchResultDto
    {
        public Guid PharmacyMedicineId { get; set; }
        public string TradeNameAr { get; set; } = string.Empty;
        public string TradeNameEn { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
    }
}
