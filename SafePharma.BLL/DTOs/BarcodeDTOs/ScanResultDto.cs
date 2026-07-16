public class ScanResultDto
{
    public Guid? MedicineId { get; set; }

    public Guid? PharmacyMedicineId { get; set; }

    public string MedicineName { get; set; } = null!;

    public decimal? Price { get; set; }

    public string BarcodeSource { get; set; } = null!;
}