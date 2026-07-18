using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class OrganManager : IOrganManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrganManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResult<IEnumerable<OrganReadDto>>> GetAll()
        {
            var organs = await _unitOfWork.OrganRepository.GetAll();

            var result = organs.Select(x => new OrganReadDto
            {
                Id = x.Id,
                NameEn = x.NameEn,
                NameAr = x.NameAr
            });

            return GeneralResult<IEnumerable<OrganReadDto>>
                .SuccessResult(result);
        }

        public async Task<GeneralResult<OrganReadDto>> Create(CreateOrganDto dto)
        {
            var organ = new Organ
            {
                Id = Guid.NewGuid(),
                NameEn = dto.NameEn.Trim(),
                NameAr = dto.NameAr.Trim()
            };

            var organs = await _unitOfWork.OrganRepository.GetAll();

            if (organs.Any(x =>
                x.NameEn.ToLower() == dto.NameEn.Trim().ToLower()
                || x.NameAr == dto.NameAr.Trim()))
            {
                return GeneralResult<OrganReadDto>.FailResult("Organ already exists.");
            }

            _unitOfWork.OrganRepository.Add(organ);

            await _unitOfWork.SaveAsync();

            return GeneralResult<OrganReadDto>.SuccessResult(
                new OrganReadDto
                {
                    Id = organ.Id,
                    NameEn = organ.NameEn,
                    NameAr = organ.NameAr
                });
        }
    }
}