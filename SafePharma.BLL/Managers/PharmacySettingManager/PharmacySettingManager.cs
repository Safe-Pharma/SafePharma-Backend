using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class PharmacySettingManager : IPharmacySettingManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public PharmacySettingManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PharmacySettingsReadDto?> GetSettings()
        {
            var settings = await _unitOfWork.PharmacySettingRepository.GetSettings();

            if (settings is null) return null;

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
            return settingsDto;
        }

        public async Task<PharmacySettingsUpdateDto?> updatePharamcySettings(PharmacySettingsUpdateDto dto)
        {
            var entity = await _unitOfWork.PharmacySettingRepository.GetSettings();

            if (entity is null) return null;

            entity.Name = dto.Name;
            entity.LogoUrl = dto.LogoUrl;
            entity.Street = dto.Street;
            entity.City = dto.City;
            entity.Governorate = dto.Governorate;
            entity.Phone = dto.Phone;
            entity.TaxRegistrationNumber = dto.TaxRegistrationNumber;
            entity.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveAsync();

            return dto;
        }
    }
}
