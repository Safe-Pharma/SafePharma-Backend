namespace SafePharma.DAL
{
    public class PharmacyMedicineTax
    {
        public Guid PharmacyMedicineId { get; set; }
        public PharmacyMedicine PharmacyMedicine { get; set; } = null!;

        public Guid TaxId { get; set; }
        public Tax Tax { get; set; } = null!;
    }
}