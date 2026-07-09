namespace SafePharma.BLL
{
    public class LinkExistingMedicineDto
    {
        public Guid MedicineId { get; set; }
        public Guid TaxId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int MinStockLevel { get; set; }
    }
}