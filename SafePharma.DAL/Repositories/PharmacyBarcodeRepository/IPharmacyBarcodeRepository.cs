using SafePharma.DAL;

public interface IPharmacyBarcodeRepository : IGenircRepository<PharmacyBarcode>
{
    Task<PharmacyBarcode?> GetByBarcodeAsync(string barcode, Guid pharmacyId);

    Task<bool> ExistsAsync(string barcode, Guid pharmacyMedicineId);
}