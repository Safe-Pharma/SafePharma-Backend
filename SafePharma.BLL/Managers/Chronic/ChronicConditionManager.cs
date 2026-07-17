using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL;

public class ChronicConditionManager : IChronicConditionManager
{
    private readonly IUnitOfWork _unitOfWork;

    public ChronicConditionManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GeneralResult<IEnumerable<ChronicConditionReadDto>>> GetAll()
    {
        var conditions = await _unitOfWork.ChronicConditionRepository.GetAll();

        var result = conditions.Select(c => new ChronicConditionReadDto
        {
            Id = c.Id,
            NameEn = c.NameEn,
            NameAr = c.NameAr
        });

        return GeneralResult<IEnumerable<ChronicConditionReadDto>>.SuccessResult(result);
    }

    public async Task<GeneralResult<ChronicConditionReadDto>> Create(CreateChronicConditionDto dto)
    {
        var condition = new ChronicCondition
        {
            Id = Guid.NewGuid(),
            NameEn = dto.NameEn,
            NameAr = dto.NameAr
        };

        _unitOfWork.ChronicConditionRepository.Add(condition);

        await _unitOfWork.SaveAsync();

        return GeneralResult<ChronicConditionReadDto>.SuccessResult(new ChronicConditionReadDto
        {
            Id = condition.Id,
            NameEn = condition.NameEn,
            NameAr = condition.NameAr
        });
    }
}