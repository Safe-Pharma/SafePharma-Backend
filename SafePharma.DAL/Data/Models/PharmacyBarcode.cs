namespace SafePharma.DAL
{
    public class PharmacyBarcode
    {
        public Guid Id { get; set; }

        public Guid PharmacyMedicineId { get; set; }

        public string Barcode { get; set; } = null!;

        public bool IsPrimary { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual PharmacyMedicine PharmacyMedicine { get; set; } = null!;
    }
}
