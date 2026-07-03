using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class TaxManager : ITaxManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaxManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TaxDto>> GetAllTaxes(string? search = null)
        {
            var taxes = await _unitOfWork.TaxRepository.Search(search);
            return taxes.Select(t => t.ToDto());
        }

        public async Task<TaxDto?> GetTaxById(Guid id)
        {
            var tax = await _unitOfWork.TaxRepository.GetById(id);
            return tax?.ToDto();
        }

        public async Task<TaxStatsDto> GetStats()
        {
            var taxes = (await _unitOfWork.TaxRepository.GetAll()).ToList();

            var active = taxes.Count(t => t.Status == TaxStatus.Active);

            return new TaxStatsDto
            {
                TotalTaxes = taxes.Count,
                Active = active,
                Inactive = taxes.Count - active,
                AverageRate = taxes.Count == 0 ? 0 : Math.Round(taxes.Average(t => t.Rate), 1)
            };
        }

        public async Task<TaxCreateResult> CreateTax(TaxCreateDto dto)
        {
            if (await _unitOfWork.TaxRepository.NameExists(dto.Name))
            {
                return new TaxCreateResult { DuplicateName = true };
            }

            var entity = dto.ToEntity();
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TaxRepository.Add(entity);
            await _unitOfWork.SaveAsync();

            return new TaxCreateResult { Tax = entity.ToDto() };
        }

        public async Task<TaxUpdateResult> UpdateTax(Guid id, TaxUpdateDto dto)
        {
            var entity = await _unitOfWork.TaxRepository.GetById(id);
            if (entity is null)
            {
                return new TaxUpdateResult { NotFound = true };
            }

            if (await _unitOfWork.TaxRepository.NameExists(dto.Name, id))
            {
                return new TaxUpdateResult { DuplicateName = true };
            }

            dto.ApplyTo(entity);
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return new TaxUpdateResult { Tax = entity.ToDto() };
        }

        public async Task<bool> DeleteTax(Guid id)
        {
            var entity = await _unitOfWork.TaxRepository.GetById(id);
            if (entity is null)
            {
                return false;
            }

            _unitOfWork.TaxRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<TaxDto?> ToggleStatus(Guid id)
        {
            var entity = await _unitOfWork.TaxRepository.GetById(id);
            if (entity is null)
            {
                return null;
            }

            entity.Status = entity.Status == TaxStatus.Active ? TaxStatus.Inactive : TaxStatus.Active;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return entity.ToDto();
        }
    }
}
