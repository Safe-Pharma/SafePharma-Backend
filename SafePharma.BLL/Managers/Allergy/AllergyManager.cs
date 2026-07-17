using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL;

public class AllergyManager : IAllergyManager
{
    private readonly IUnitOfWork _unitOfWork;

    public AllergyManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GeneralResult<IEnumerable<AllergyReadDto>>> GetAll()
    {
        var allergies = await _unitOfWork.AllergyRepository.GetAll();

        var result = allergies.Select(a => new AllergyReadDto
        {
            Id = a.Id,
            NameEn = a.NameEn,
            NameAr = a.NameAr
        });

        return GeneralResult<IEnumerable<AllergyReadDto>>.SuccessResult(result);
    }

    public async Task<GeneralResult<AllergyReadDto>> Create(CreateAllergyDto dto)
    {
        var allergy = new Allergy
        {
            Id = Guid.NewGuid(),
            NameEn = dto.NameEn,
            NameAr = dto.NameAr
        };

        _unitOfWork.AllergyRepository.Add(allergy);

        await _unitOfWork.SaveAsync();

        return GeneralResult<AllergyReadDto>.SuccessResult(new AllergyReadDto
        {
            Id = allergy.Id,
            NameEn = allergy.NameEn,
            NameAr = allergy.NameAr
        });
    }
}