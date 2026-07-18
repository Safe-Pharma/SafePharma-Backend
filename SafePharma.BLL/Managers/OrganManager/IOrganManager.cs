using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IOrganManager
    {
        Task<GeneralResult<IEnumerable<OrganReadDto>>> GetAll();

        Task<GeneralResult<OrganReadDto>> Create(CreateOrganDto dto);
    }
}