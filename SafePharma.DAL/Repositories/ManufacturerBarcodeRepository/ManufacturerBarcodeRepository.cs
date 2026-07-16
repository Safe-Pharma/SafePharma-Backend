using Microsoft.EntityFrameworkCore;
using SafePharma.DAL;

public class ManufacturerBarcodeRepository
    : GenircRepository<ManufacturerBarcode>, IManufacturerBarcodeRepository
{
    public ManufacturerBarcodeRepository(AppDbContext db) : base(db)
    {
    }

    public async Task<ManufacturerBarcode?> GetByBarcodeAsync(string barcode)
    {
        return await _db.Set<ManufacturerBarcode>()
            .Include(x => x.Medicine)
            .FirstOrDefaultAsync(x => x.Barcode == barcode);
    }

    public async Task<bool> ExistsAsync(string barcode)
    {
        return await _db.Set<ManufacturerBarcode>()
            .AnyAsync(x => x.Barcode == barcode);
    }
}