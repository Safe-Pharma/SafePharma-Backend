namespace SafePharma.DAL
{
    public class StockAggregate
    {
        public Guid PharmacyMedicineId { get; set; }
        public int TotalStock { get; set; }
        public int AvailableQuantity { get; set; }
        public int BatchCount { get; set; }
        public int ExpiringSoon { get; set; }
    }
}