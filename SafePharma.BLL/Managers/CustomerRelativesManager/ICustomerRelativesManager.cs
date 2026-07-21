using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public interface ICustomerRelativesManager
    {
        Task<GeneralResult> CreateRelation(CustomerRelativeCreateDto dto);
        Task<GeneralResult<IEnumerable<CustomerRelativeReadDto>>> GetRelations(Guid id);

    }
}