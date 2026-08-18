using SafePharma.BLL.DTOs.PharmacyDtos;
using SafePharma.Common;

namespace SafePharma.BLL.Managers.PharmacyManager
{
    public interface IPharmacyManager
    {
        Task<GeneralResult<IEnumerable<PharmacyReadDto>>> GetAllPharmacies();
        Task<GeneralResult> UpdatePharmacyStatus(Guid id);

    }
}