using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class OrganImpairmentLevelManager : IOrganImpairmentLevelManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrganImpairmentLevelManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResult<IEnumerable<OrganImpairmentLevelReadDto>>> GetAll()
        {
            var levels = await _unitOfWork.OrganImpairmentLevelRepository.GetAll();

            var result = levels.Select(x => new OrganImpairmentLevelReadDto
            {
                Id = x.Id,
                NameEn = x.NameEn,
                NameAr = x.NameAr
            });

            return GeneralResult<IEnumerable<OrganImpairmentLevelReadDto>>
                .SuccessResult(result);
        }

        public async Task<GeneralResult<OrganImpairmentLevelReadDto>> Create(CreateOrganImpairmentLevelDto dto)
        {
            var levels = await _unitOfWork.OrganImpairmentLevelRepository.GetAll();

            if (levels.Any(x =>
                x.NameEn.Trim().ToLower() == dto.NameEn.Trim().ToLower()
                || x.NameAr.Trim() == dto.NameAr.Trim()))
            {
                return GeneralResult<OrganImpairmentLevelReadDto>
                    .FailResult("Organ impairment level already exists.");
            }

            var level = new OrganImpairmentLevel
            {
                Id = Guid.NewGuid(),
                NameEn = dto.NameEn.Trim(),
                NameAr = dto.NameAr.Trim()
            };

            _unitOfWork.OrganImpairmentLevelRepository.Add(level);

            await _unitOfWork.SaveAsync();

            return GeneralResult<OrganImpairmentLevelReadDto>
                .SuccessResult(new OrganImpairmentLevelReadDto
                {
                    Id = level.Id,
                    NameEn = level.NameEn,
                    NameAr = level.NameAr
                });
        }
    }
}