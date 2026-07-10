namespace SafePharma.DAL.Data.Models
{
    public class ManufacturerBarcode
    {
        public Guid Id { get; set; }

        public Guid MedicineId { get; set; }

        public string Barcode { get; set; } = null!;

        public bool IsPrimary { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Medicine Medicine { get; set; } = null!;
    }
}
