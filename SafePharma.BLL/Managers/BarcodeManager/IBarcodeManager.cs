using SafePharma.Common;

public interface IBarcodeManager
{
    Task<GeneralResult> AddManufacturerBarcodeAsync(AddManufacturerBarcodeDto dto);

    Task<GeneralResult> AddPharmacyBarcodeAsync(AddPharmacyBarcodeDto dto);
    Task<GeneralResult<ScanResultDto>> ScanAsync(ScanBarcodeDto dto);
}