using Microsoft.EntityFrameworkCore;
using SafePharma.DAL;

public class PharmacyBarcodeRepository
    : GenircRepository<PharmacyBarcode>, IPharmacyBarcodeRepository
{
    public PharmacyBarcodeRepository(AppDbContext db) : base(db)
    {
    }

    public async Task<PharmacyBarcode?> GetByBarcodeAsync(string barcode, Guid pharmacyId)
    {
        return await _db.Set<PharmacyBarcode>()
            .Include(x => x.PharmacyMedicine)
                .ThenInclude(pm => pm.Medicine)
            .FirstOrDefaultAsync(x =>
                x.Barcode == barcode &&
                x.PharmacyMedicine.PharmacyId == pharmacyId);
    }

    public async Task<bool> ExistsAsync(string barcode, Guid pharmacyMedicineId)
    {
        return await _db.Set<PharmacyBarcode>()
            .AnyAsync(x =>
                x.Barcode == barcode &&
                x.PharmacyMedicineId == pharmacyMedicineId);
    }
}