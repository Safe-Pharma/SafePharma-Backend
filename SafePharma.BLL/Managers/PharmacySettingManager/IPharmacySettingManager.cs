using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public interface IPharmacySettingManager
    {
        Task<GeneralResult<PharmacySettingsReadDto?>> GetSettings();
        Task<GeneralResult<PharmacySettingsUpdateDto?>> updatePharamcySettings(PharmacySettingsUpdateDto dto);
    }
}