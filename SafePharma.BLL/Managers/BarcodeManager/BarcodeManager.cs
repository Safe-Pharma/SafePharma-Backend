using FluentValidation;
using SafePharma.Common;
using SafePharma.DAL;
using SafePharma.DAL.Data.Models;

public class BarcodeManager : IBarcodeManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUser;
    private readonly IValidator<AddManufacturerBarcodeDto> _manufacturerValidator;
    private readonly IValidator<AddPharmacyBarcodeDto> _pharmacyValidator;

    public BarcodeManager(
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUser,
        IValidator<AddManufacturerBarcodeDto> manufacturerValidator,
        IValidator<AddPharmacyBarcodeDto> pharmacyValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _manufacturerValidator = manufacturerValidator;
        _pharmacyValidator = pharmacyValidator;
    }

    public async Task<GeneralResult> AddManufacturerBarcodeAsync(AddManufacturerBarcodeDto dto)
    {
        var validation = await _manufacturerValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));
            return GeneralResult.FailResult(errors);
        }

        var barcode = dto.Barcode.Trim().ToUpper();
        var exists = await _unitOfWork.ManufacturerBarcodeRepository.ExistsAsync(barcode);
        if (exists)
        {
            return GeneralResult.FailResult("This Barcode Already Exists");
        }

        var medicine = await _unitOfWork.MedicineRepository.GetById(dto.MedicineId);
        if (medicine == null)
        {
            return GeneralResult.FailResult("Medicine Not Found");
        }

        if (dto.IsPrimary)
        {
            var existingPrimary = await _unitOfWork.ManufacturerBarcodeRepository
                .GetAllWithException(x => x.MedicineId == dto.MedicineId && x.IsPrimary);

            foreach (var item in existingPrimary)
            {
                item.IsPrimary = false;
                _unitOfWork.ManufacturerBarcodeRepository.Update(item);
            }
        }

        var record = new ManufacturerBarcode
        {
            Id = Guid.NewGuid(),
            MedicineId = dto.MedicineId,
            Barcode = barcode,
            IsPrimary = dto.IsPrimary,
            CreatedAt = DateTime.UtcNow
        };
        _unitOfWork.ManufacturerBarcodeRepository.Add(record);

        await _unitOfWork.SaveAsync();
        return GeneralResult.SuccessResult("Barcode is Added");
    }

    public async Task<GeneralResult> AddPharmacyBarcodeAsync(AddPharmacyBarcodeDto dto)
    {
        var pharmacyId = _currentUser.PharmacyId;
        if (pharmacyId == Guid.Empty)
        {
            return GeneralResult<ScanResultDto>.FailResult("Pharmacy context not found");
        }
        var validation = await _pharmacyValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));
            return GeneralResult.FailResult(errors);
        }

        string barcode;
        if (string.IsNullOrWhiteSpace(dto.Barcode))
        {
            do
            {
                barcode = GenerateBarcode();
            }
            while (await _unitOfWork.PharmacyBarcodeRepository
                .ExistsAsync(barcode, dto.PharmacyMedicineId));
        }
        else
        {
            barcode = dto.Barcode.Trim().ToUpper();
        }

        var exists = await _unitOfWork.PharmacyBarcodeRepository
            .ExistsAsync(barcode, dto.PharmacyMedicineId);
        if (exists)
        {
            return GeneralResult.FailResult("Barcode already exists for this medicine");
        }

        var pharmacyMedicine = await _unitOfWork.PharmacyMedicineRepository
        .GetById(dto.PharmacyMedicineId);

        if (pharmacyMedicine == null || pharmacyMedicine.PharmacyId != pharmacyId)
        {
            return GeneralResult.FailResult("Pharmacy medicine not found in this Pharmacy");
        }
        if (dto.IsPrimary)
        {
            var existingPrimary = await _unitOfWork.PharmacyBarcodeRepository
                .GetAllWithException(x =>
                    x.PharmacyMedicineId == dto.PharmacyMedicineId && x.IsPrimary);

            foreach (var item in existingPrimary)
            {
                item.IsPrimary = false;
                _unitOfWork.PharmacyBarcodeRepository.Update(item);
            }
        }

        var record = new PharmacyBarcode
        {
            Id = Guid.NewGuid(),
            PharmacyMedicineId = dto.PharmacyMedicineId,
            Barcode = barcode,
            IsPrimary = dto.IsPrimary,
            CreatedAt = DateTime.UtcNow
        };
        _unitOfWork.PharmacyBarcodeRepository.Add(record);

        await _unitOfWork.SaveAsync();
        return GeneralResult.SuccessResult("Barcode Is Added");
    }

    public async Task<GeneralResult<ScanResultDto>> ScanAsync(ScanBarcodeDto dto)
    {
        var pharmacyId = _currentUser.PharmacyId;
        if (pharmacyId == Guid.Empty)
        {
            return GeneralResult<ScanResultDto>.FailResult("Pharmacy context not found");
        }

        var barcode = dto.Barcode.Trim().ToUpper();

        var pharmacyBarcode = await _unitOfWork.PharmacyBarcodeRepository
            .GetByBarcodeAsync(barcode, pharmacyId);

        if (pharmacyBarcode != null)
        {
            var result = new ScanResultDto
            {
                MedicineId = pharmacyBarcode.PharmacyMedicine.MedicineId,
                PharmacyMedicineId = pharmacyBarcode.PharmacyMedicineId,
                MedicineName = pharmacyBarcode.PharmacyMedicine.Medicine.TradeNameEn,
                Price = pharmacyBarcode.PharmacyMedicine.SellingPrice,
                BarcodeSource = "PharmacyBarcode"
            };
            return GeneralResult<ScanResultDto>.SuccessResult(result);
        }

        // 2. Manufacturer barcode fallback
        var manufacturerBarcode = await _unitOfWork.ManufacturerBarcodeRepository
            .GetByBarcodeAsync(barcode);

        if (manufacturerBarcode != null)
        {
            var pharmacyMedicine = await _unitOfWork.PharmacyMedicineRepository
                .GetByMedicineAndPharmacy(manufacturerBarcode.MedicineId, pharmacyId);

            var result = new ScanResultDto
            {
                MedicineId = manufacturerBarcode.MedicineId,
                PharmacyMedicineId = pharmacyMedicine?.Id,
                MedicineName = manufacturerBarcode.Medicine.TradeNameEn,
                Price = pharmacyMedicine?.SellingPrice,
                BarcodeSource = "ManufacturerBarcode"
            };
            return GeneralResult<ScanResultDto>.SuccessResult(result);
        }

        return GeneralResult<ScanResultDto>.FailResult("Barcode not found");
    }




    private string GenerateBarcode()
    {
        return $"PH-{Guid.NewGuid().ToString().Substring(0, 8)}".ToUpper();
    }
}