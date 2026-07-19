using SafePharma.Common;
using SafePharma.Common.Enums;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class SupplierManager : ISupplierManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditManager _auditManager;
        public SupplierManager(IUnitOfWork unitOfWork, IAuditManager auditManager)
        {
            _unitOfWork = unitOfWork;
            _auditManager = auditManager;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliers(Guid pharmacyId, string? search = null)
        {
            var suppliers = await _unitOfWork.SupplierRepository.Search(pharmacyId, search);
            return suppliers.Select(s => s.ToDto());
        }

        public async Task<SupplierDto?> GetSupplierById(Guid pharmacyId, Guid id)
        {
            var supplier = await GetOwnedSupplier(pharmacyId, id);
            return supplier?.ToDto();
        }

        public async Task<SupplierStatsDto> GetStats(Guid pharmacyId)
        {
            var suppliers = (await _unitOfWork.SupplierRepository.GetAllForPharmacy(pharmacyId)).ToList();
            var paymentsCount = await _unitOfWork.SupplierPaymentRepository.CountForPharmacy(pharmacyId);

            var active = suppliers.Count(s => s.Status == SupplierStatus.Active);

            return new SupplierStatsDto
            {
                TotalSuppliers = suppliers.Count,
                Active = active,
                Inactive = suppliers.Count - active,
                CountriesCount = suppliers.Select(s => s.CountryId).Distinct().Count(),
                PaymentsRecorded = paymentsCount
            };
        }

        public async Task<SupplierCreateResult> CreateSupplier(Guid pharmacyId, SupplierCreateDto dto)
        {
            if (await _unitOfWork.SupplierRepository.NameExists(pharmacyId, dto.Name))
            {
                return new SupplierCreateResult { DuplicateName = true };
            }

            var entity = dto.ToEntity();
            entity.Id = Guid.NewGuid();
            entity.PharmacyId = pharmacyId;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.SupplierRepository.Add(entity);
            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.SupplierRepository.GetByIdWithCountry(entity.Id);
            await _auditManager.CreateAudit(saved, null, ActionsEnum.Create);
            return new SupplierCreateResult { Supplier = saved!.ToDto() };
        }

        public async Task<SupplierUpdateResult> UpdateSupplier(Guid pharmacyId, Guid id, SupplierUpdateDto dto)
        {
            var entity = await GetOwnedSupplier(pharmacyId, id);
            if (entity is null)
            {
                return new SupplierUpdateResult { NotFound = true };
            }

            if (await _unitOfWork.SupplierRepository.NameExists(pharmacyId, dto.Name, id))
            {
                return new SupplierUpdateResult { DuplicateName = true };
            }
            var oldEntity = new Supplier
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactPerson = entity.ContactPerson,
                Email = entity.Email,
                Address = entity.Address,
                CountryId = entity.CountryId,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                PharmacyId = entity.PharmacyId
            };
            dto.ApplyTo(entity);
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.SupplierRepository.GetByIdWithCountry(entity.Id);
            await _auditManager.CreateAudit(saved, oldEntity, ActionsEnum.Update);
            return new SupplierUpdateResult { Supplier = saved!.ToDto() };
        }

        public async Task<bool> DeleteSupplier(Guid pharmacyId, Guid id)
        {
            var entity = await GetOwnedSupplier(pharmacyId, id);
            if (entity is null)
            {
                return false;
            }

            _unitOfWork.SupplierRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            var deleted = await _unitOfWork.SupplierRepository.GetByIdWithCountry(entity.Id);
            //await _auditManager.CreateAudit(deleted, entity, ActionsEnum.Delete);
            return true;
        }

        public async Task<SupplierDto?> ToggleStatus(Guid pharmacyId, Guid id)
        {
            var entity = await GetOwnedSupplier(pharmacyId, id);
            if (entity is null)
            {
                return null;
            }

            entity.Status = entity.Status == SupplierStatus.Active ? SupplierStatus.Inactive : SupplierStatus.Active;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return entity.ToDto();
        }

        
        private async Task<Supplier?> GetOwnedSupplier(Guid pharmacyId, Guid id)
        {
            var entity = await _unitOfWork.SupplierRepository.GetByIdWithCountry(id);
            return entity is null || entity.PharmacyId != pharmacyId ? null : entity;
        }
    }
}