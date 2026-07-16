using SafePharma.DAL;

public interface IManufacturerBarcodeRepository : IGenircRepository<ManufacturerBarcode>
{
    Task<ManufacturerBarcode?> GetByBarcodeAsync(string barcode);

    Task<bool> ExistsAsync(string barcode);
}