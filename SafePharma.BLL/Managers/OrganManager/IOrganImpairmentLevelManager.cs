using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IOrganImpairmentLevelManager
    {
        Task<GeneralResult<IEnumerable<OrganImpairmentLevelReadDto>>> GetAll();

        Task<GeneralResult<OrganImpairmentLevelReadDto>> Create(CreateOrganImpairmentLevelDto dto);
    }
}