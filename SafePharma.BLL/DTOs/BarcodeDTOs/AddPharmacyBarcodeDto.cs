public class AddPharmacyBarcodeDto
{
    public Guid PharmacyMedicineId { get; set; }

    public string? Barcode { get; set; } = null!;

    public bool IsPrimary { get; set; }
}