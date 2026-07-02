
namespace SafePharma.DAL
{
    public interface IPharmacySettingRepository : IGenircRepository<PharmacySettings>
    {
        Task<PharmacySettings?> GetSettings();
    }
}