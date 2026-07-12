using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SafePharma.Common.Enums;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class TaxManager : ITaxManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditManager _auditManager;

        public TaxManager(IUnitOfWork unitOfWork , IAuditManager auditManager)
        {
            _unitOfWork = unitOfWork;
            _auditManager = auditManager;
        }

        public async Task<IEnumerable<TaxDto>> GetAllTaxes(Guid pharmacyId, string? search = null)
        {
            var taxes = await _unitOfWork.TaxRepository.Search(pharmacyId, search);
            return taxes.Select(t => t.ToDto());
        }

        public async Task<TaxDto?> GetTaxById(Guid pharmacyId, Guid id)
        {
            var tax = await GetOwnedTax(pharmacyId, id);
            return tax?.ToDto();
        }

        public async Task<TaxStatsDto> GetStats(Guid pharmacyId)
        {
            var taxes = (await _unitOfWork.TaxRepository.GetAllForPharmacy(pharmacyId)).ToList();

            var active = taxes.Count(t => t.Status == TaxStatus.Active);

            return new TaxStatsDto
            {
                TotalTaxes = taxes.Count,
                Active = active,
                Inactive = taxes.Count - active,
                AverageRate = taxes.Count == 0 ? 0 : Math.Round(taxes.Average(t => t.Rate), 1)
            };
        }

        public async Task<TaxCreateResult> CreateTax(Guid pharmacyId, TaxCreateDto dto)
        {
            if (await _unitOfWork.TaxRepository.NameExists(pharmacyId, dto.Name))
            {
                return new TaxCreateResult { DuplicateName = true };
            }

            var entity = dto.ToEntity();
            entity.Id = Guid.NewGuid();
            entity.PharmacyId = pharmacyId;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TaxRepository.Add(entity);
            await _unitOfWork.SaveAsync();
            await _auditManager.CreateAudit(entity,null, ActionsEnum.Create);
            return new TaxCreateResult { Tax = entity.ToDto() };
        }

        public async Task<TaxUpdateResult> UpdateTax(Guid pharmacyId, Guid id, TaxUpdateDto dto)
        {
            var new_entity = await GetOwnedTax(pharmacyId, id);

            if (new_entity is null)
            {
                return new TaxUpdateResult { NotFound = true };
            }

            if (await _unitOfWork.TaxRepository.NameExists(pharmacyId, dto.Name, id))
            {
                return new TaxUpdateResult { DuplicateName = true };
            }
            var old_entity = new Tax
            {
                Id = new_entity.Id,
                Name = new_entity.Name,
                Rate = new_entity.Rate,
                Status = new_entity.Status,
                PharmacyId = new_entity.PharmacyId,
                CreatedAt = new_entity.CreatedAt,
                UpdatedAt = new_entity.UpdatedAt
            };
            dto.ApplyTo(new_entity);
            new_entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();
            await _auditManager.CreateAudit(new_entity, old_entity, ActionsEnum.Update);
            return new TaxUpdateResult { Tax = new_entity.ToDto() };
        }

        public async Task<bool> DeleteTax(Guid pharmacyId, Guid id)
        {
            var entity = await GetOwnedTax(pharmacyId, id);
            if (entity is null)
            {
                return false;
            }

            _unitOfWork.TaxRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
           // await _auditManager.CreateAudit(null, entity, ActionsEnum.Delete);
            return true;
        }

        public async Task<TaxDto?> ToggleStatus(Guid pharmacyId, Guid id)
        {
            var entity = await GetOwnedTax(pharmacyId, id);
            if (entity is null)
            {
                return null;
            }

            entity.Status = entity.Status == TaxStatus.Active ? TaxStatus.Inactive : TaxStatus.Active;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return entity.ToDto();
        }

        
        private async Task<Tax?> GetOwnedTax(Guid pharmacyId, Guid id)
        {
            var entity = await _unitOfWork.TaxRepository.GetById(id);
            return entity is null || entity.PharmacyId != pharmacyId ? null : entity;
        }
    }
}