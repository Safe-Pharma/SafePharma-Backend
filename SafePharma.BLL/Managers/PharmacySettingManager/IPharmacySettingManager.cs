using SafePharma.DAL;

namespace SafePharma.BLL
{
    public interface IPharmacySettingManager
    {
        Task<PharmacySettings?> GetSettings();
        Task<PharmacySettingsUpdateDto> updatePharamcySettings(PharmacySettingsUpdateDto dto);
    }
}