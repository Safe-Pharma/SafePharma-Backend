using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public interface IPharmacySettingManager
    {
        Task<GeneralResult<PharmacySettingsReadDto?>> GetSettings(Guid pharmacyId);
        Task<GeneralResult<PharmacySettingsUpdateDto?>> updatePharamcySettings(PharmacySettingsUpdateDto dto, Guid pharmacyId);
    }
}