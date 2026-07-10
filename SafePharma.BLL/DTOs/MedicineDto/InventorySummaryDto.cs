namespace SafePharma.BLL
{
    public class InventorySummaryDto
    {
        public int TotalStock { get; set; }
        public int AvailableQuantity { get; set; }
        public int NumberOfBatches { get; set; }
        public int ExpiringSoon { get; set; }
        public string StockStatus { get; set; } = "InStock"; // "InStock" | "Low" | "Out"
    }
}