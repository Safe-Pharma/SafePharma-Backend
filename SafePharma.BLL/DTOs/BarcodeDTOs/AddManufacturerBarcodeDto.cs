public class AddManufacturerBarcodeDto
{
    public Guid MedicineId { get; set; }

    public string Barcode { get; set; } = null!;

    public bool IsPrimary { get; set; }
}