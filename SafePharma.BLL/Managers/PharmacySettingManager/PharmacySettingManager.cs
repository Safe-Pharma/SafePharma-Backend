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
            var settings = await _unitOfWork.PharmacySettingRepository.GetSettingsByPharmacyId(pharmacyId);

            if (settings is null) return GeneralResult<PharmacySettingsReadDto?>.NotFound();

            PharmacySettingsReadDto settingsDto = new PharmacySettingsReadDto()
            {
                Name = settings.Name,
                LogoUrl = settings.LogoUrl,
                Street = settings.Street,
                City = settings.City,
                Governorate = settings.Governorate,
                Phone = settings.Phone,
                TaxRegistrationNumber = settings.TaxRegistrationNumber
            };
            return GeneralResult<PharmacySettingsReadDto?>.SuccessResult(settingsDto);
        }

        public async Task<GeneralResult<PharmacySettingsUpdateDto?>> updatePharamcySettings(PharmacySettingsUpdateDto dto, Guid pharmacyId)
        {
            var entity = await _unitOfWork.PharmacySettingRepository.GetSettingsByPharmacyId(pharmacyId);

            if (entity is null) return GeneralResult<PharmacySettingsUpdateDto?>.NotFound();

            entity.Name = dto.Name;
            entity.Street = dto.Street;
            entity.City = dto.City;
            entity.Governorate = dto.Governorate;
            entity.Phone = dto.Phone;
            entity.TaxRegistrationNumber = dto.TaxRegistrationNumber;

            entity.UpdatedAt = DateTime.UtcNow;

            if (dto.LogoFile != null)
            {
                var imageUrl = await _cloudinary.UploadImageAsync(dto.LogoFile);
                entity.LogoUrl = imageUrl;
            }

            await _unitOfWork.SaveAsync();

            return GeneralResult<PharmacySettingsUpdateDto?>.SuccessResult(dto);

        }
    }
}
