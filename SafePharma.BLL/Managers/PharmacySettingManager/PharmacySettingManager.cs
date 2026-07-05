using SafePharma.Common;
using SafePharma.DAL;


namespace SafePharma.BLL
{
    public class PharmacySettingManager : IPharmacySettingManager
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ICloudinaryService _cloudinary;
        public PharmacySettingManager(IUnitOfWork unitOfWork, ICloudinaryService cloudinary)
        {
            _unitOfWork = unitOfWork;
            _cloudinary = cloudinary;
        }

        public async Task<GeneralResult<PharmacySettingsReadDto?>> GetSettings(Guid pharmacyId)
        {
            var pharmacy = await _unitOfWork.PharmacyRepository.GetById(pharmacyId);

            if (pharmacy is null) return GeneralResult<PharmacySettingsReadDto?>.NotFound();

            PharmacySettingsReadDto settingsDto = new PharmacySettingsReadDto()
            {
                Name = pharmacy.Name,
                LogoUrl = pharmacy.LogoUrl,
                Address = pharmacy.Address,
                City = pharmacy.City,
                Country = pharmacy.Country,
                Phone = pharmacy.Phone,
                TaxRegistrationNumber = pharmacy.TaxNumber
            };
            return GeneralResult<PharmacySettingsReadDto?>.SuccessResult(settingsDto);
        }

        public async Task<GeneralResult<PharmacySettingsUpdateDto?>> updatePharamcySettings(PharmacySettingsUpdateDto dto, Guid pharmacyId)
        {
            var pharmacy = await _unitOfWork.PharmacyRepository.GetById(pharmacyId);

            if (pharmacy is null) return GeneralResult<PharmacySettingsUpdateDto?>.NotFound();

            pharmacy.Name = dto.Name;
            pharmacy.Address = dto.Address;
            pharmacy.City = dto.City;
            pharmacy.Country = dto.Country;
            pharmacy.Phone = dto.Phone;
            pharmacy.TaxNumber = dto.TaxRegistrationNumber;

            pharmacy.UpdatedAt = DateTime.UtcNow;

            if (dto.LogoFile != null)
            {
                var imageUrl = await _cloudinary.UploadImageAsync(dto.LogoFile);
                pharmacy.LogoUrl = imageUrl;
            }

            await _unitOfWork.SaveAsync();

            return GeneralResult<PharmacySettingsUpdateDto?>.SuccessResult(dto);

        }
    }
}
