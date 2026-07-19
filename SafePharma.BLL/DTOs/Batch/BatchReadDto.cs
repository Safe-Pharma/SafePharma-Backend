using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class BatchReadDto
    {
        public String MedeicineName { get; set; } = string.Empty;
        public String MedeicineCategory { get; set; } = string.Empty;
        public String SKU { get; set; } = string.Empty;


        public List<BatchItemDto> Batches { get; set; }

        public int BatchesCount { get; set; }
        public decimal OnHand { get; set; }
        public int MinStockLevel { get; set; }
        public string StockLevel { get; set; } = string.Empty;


    }
}
