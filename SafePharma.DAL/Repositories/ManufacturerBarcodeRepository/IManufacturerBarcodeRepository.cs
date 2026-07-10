using SafePharma.DAL;
using SafePharma.DAL.Data.Models;

public interface IManufacturerBarcodeRepository : IGenircRepository<ManufacturerBarcode>
{
    Task<ManufacturerBarcode?> GetByBarcodeAsync(string barcode);

    Task<bool> ExistsAsync(string barcode);
}