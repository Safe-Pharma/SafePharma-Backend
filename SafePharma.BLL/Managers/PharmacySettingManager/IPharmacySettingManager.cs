using SafePharma.DAL;

namespace SafePharma.BLL
{
    public interface IPharmacySettingManager
    {
        Task<PharmacySettingsReadDto?> GetSettings();
        Task<PharmacySettingsUpdateDto?> updatePharamcySettings(PharmacySettingsUpdateDto dto);
    }
}