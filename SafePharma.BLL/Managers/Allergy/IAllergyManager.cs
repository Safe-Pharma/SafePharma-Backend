using SafePharma.Common;

namespace SafePharma.BLL;

public interface IAllergyManager
{
    Task<GeneralResult<IEnumerable<AllergyReadDto>>> GetAll();

    Task<GeneralResult<AllergyReadDto>> Create(CreateAllergyDto dto);
}