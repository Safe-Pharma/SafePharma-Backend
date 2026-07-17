using SafePharma.Common;

namespace SafePharma.BLL;

public interface IChronicConditionManager
{
    Task<GeneralResult<IEnumerable<ChronicConditionReadDto>>> GetAll();

    Task<GeneralResult<ChronicConditionReadDto>> Create(CreateChronicConditionDto dto);
}