using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class CustomerManager : ICustomerManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserContext _currentUserContext;

        public CustomerManager(IUnitOfWork unitOfWork , ICurrentUserContext currentUserContext)

        {
            _unitOfWork = unitOfWork;
            _currentUserContext = currentUserContext;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomers(Guid pharmacyId, string? search = null)
        {
            var customers = (await _unitOfWork.CustomerRepository.Search(search)).ToList();
            if (customers.Count == 0) return Enumerable.Empty<CustomerDto>();

            var balancesByCustomer = (await _unitOfWork.CustomerPharmacyBalanceRepository.GetForPharmacy(pharmacyId))
                .ToDictionary(b => b.CustomerId, b => b.TotalPaid);

            return customers.Select(c =>
                c.ToDto(balancesByCustomer.TryGetValue(c.Id, out var paid) ? paid : 0m));
        }

        public async Task<CustomerDto?> GetCustomerById(Guid pharmacyId, Guid id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(id);
            if (customer is null)
            {
                return null;
            }

            var balance = await _unitOfWork.CustomerPharmacyBalanceRepository.GetByCustomerAndPharmacy(id, pharmacyId);
            return customer.ToDto(balance?.TotalPaid ?? 0m);
        }
        public async Task<GeneralResult<CustomerDto?>> GetMe(Guid id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(id);

            if (customer is null)
                return GeneralResult<CustomerDto>.NotFound("Customer Not Found");

            var dto = new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address,
                DateOfBirth = customer.DateOfBirth,
                Phone = customer.Phone,
                Email = customer.Email,
                Notes = customer.Notes
            };

            return GeneralResult<CustomerDto?>.SuccessResult(dto);
        }

        public async Task<CustomerStatsDto> GetStats(Guid pharmacyId)
        {
            var customers = (await _unitOfWork.CustomerRepository.GetAll()).ToList();
            var active = customers.Count(c => c.Status == CustomerStatus.Active);

            // Only this pharmacy's own balances count toward "total paid" here —
            // TotalPaid is per-pharmacy, not a global sum across the whole platform.
            var balances = await _unitOfWork.CustomerPharmacyBalanceRepository.GetForPharmacy(pharmacyId);

            return new CustomerStatsDto
            {
                TotalCustomers = customers.Count,
                Active = active,
                Inactive = customers.Count - active,
                TotalPaidAllCustomers = balances.Sum(b => b.TotalPaid)
            };
        }

        public async Task<CustomerCreateResult> CreateCustomer(CustomerCreateDto dto)
        {
            if (await _unitOfWork.CustomerRepository.PhoneExists(dto.Phone))
            {
                return new CustomerCreateResult { DuplicatePhone = true };
            }

            var entity = dto.ToEntity();
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.CustomerRepository.Add(entity);
            await _unitOfWork.SaveAsync();

            return new CustomerCreateResult { Customer = entity.ToDto() };
        }

        public async Task<CustomerUpdateResult> UpdateCustomer(Guid id, CustomerUpdateDto dto)
        {
            var entity = await _unitOfWork.CustomerRepository.GetById(id);
            if (entity is null)
            {
                return new CustomerUpdateResult { NotFound = true };
            }

            if (await _unitOfWork.CustomerRepository.PhoneExists(dto.Phone, id))
            {
                return new CustomerUpdateResult { DuplicatePhone = true };
            }

            dto.ApplyTo(entity);
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return new CustomerUpdateResult { Customer = entity.ToDto() };
        }

        public async Task<GeneralResult<CustomerUpdateResult>> UpdateCustomerPortal(
       Guid id,
       CustomerUpdatePortalDto dto)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(id);

            if (customer is null)
            {
                return GeneralResult<CustomerUpdateResult>.NotFound("This Customer Not Found");
            }

            dto.ApplyToFromPortal(customer);
            customer.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return GeneralResult<CustomerUpdateResult>.SuccessResult();
        }


        public async Task<bool> DeleteCustomer(Guid id)
        {
            var entity = await _unitOfWork.CustomerRepository.GetById(id);
            if (entity is null)
            {
                return false;
            }

            _unitOfWork.CustomerRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<CustomerDto?> ToggleStatus(Guid pharmacyId, Guid id)
        {
            var entity = await _unitOfWork.CustomerRepository.GetById(id);
            if (entity is null)
            {
                return null;
            }

            entity.Status = entity.Status == CustomerStatus.Active ? CustomerStatus.Inactive : CustomerStatus.Active;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            var balance = await _unitOfWork.CustomerPharmacyBalanceRepository.GetByCustomerAndPharmacy(id, pharmacyId);
            return entity.ToDto(balance?.TotalPaid ?? 0m);
        }

        public async Task<RecordCustomerPaymentResult> RecordPayment(Guid pharmacyId, Guid customerId, decimal amount)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer is null)
            {
                return new RecordCustomerPaymentResult { CustomerNotFound = true };
            }

            var balance = await _unitOfWork.CustomerPharmacyBalanceRepository.GetByCustomerAndPharmacy(customerId, pharmacyId);
            if (balance is null)
            {
                balance = new CustomerPharmacyBalance
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    PharmacyId = pharmacyId,
                    TotalPaid = amount,
                    LastPaymentAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                _unitOfWork.CustomerPharmacyBalanceRepository.Add(balance);
            }
            else
            {
                balance.TotalPaid += amount;
                balance.LastPaymentAt = DateTime.UtcNow;
                balance.UpdatedAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveAsync();

            return new RecordCustomerPaymentResult { Customer = customer.ToDto(balance.TotalPaid) };
        }

        public async Task<IEnumerable<CustomerMedicineHistoryDto>?> GetMedicineHistory(Guid customerId, bool? isActive = null)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer is null)
            {
                return null;
            }

            var history = await _unitOfWork.CustomerMedicineHistoryRepository.GetForCustomer(customerId, isActive);
            return history.Select(h => h.ToDto());
        }

        public async Task<AddCustomerMedicineHistoryResult> AddMedicineHistory(Guid customerId, CreateCustomerMedicineHistoryDto dto)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer is null)
            {
                return new AddCustomerMedicineHistoryResult { CustomerNotFound = true };
            }

            // If a global medicine was picked, make sure it actually exists in the catalog.
            // If not (dto.MedicineId is null), the validator already required ScientificName —
            // the pharmacist's free-text entry — so no lookup is needed.
            if (dto.MedicineId is not null)
            {
                var medicine = await _unitOfWork.MedicineRepository.GetById(dto.MedicineId.Value);
                if (medicine is null || !medicine.IsGlobal)
                {
                    return new AddCustomerMedicineHistoryResult { MedicineNotFound = true };
                }
            }

            // One record per medicine per customer (matched by MedicineId, or by
            // ScientificName for manual entries) — adding the same medicine again
            // UPDATES the existing record (e.g. re-activates it) instead of creating
            // a duplicate row.
            var existing = await _unitOfWork.CustomerMedicineHistoryRepository
                .FindDuplicate(customerId, dto.MedicineId, dto.ScientificName);

            CustomerMedicineHistory entity;
            var wasUpdated = existing is not null;
            if (existing is not null)
            {
                existing.TradeName = dto.TradeName;
                existing.PurchaseDate = dto.PurchaseDate ?? DateTime.UtcNow;
                existing.Quantity = dto.Quantity;
                existing.IsActive = dto.IsActive;
                existing.Notes = dto.Notes;
                existing.UpdatedAt = DateTime.UtcNow;
                entity = existing;
            }
            else
            {
                entity = dto.ToEntity();
                entity.Id = Guid.NewGuid();
                entity.CustomerId = customerId;
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.CustomerMedicineHistoryRepository.Add(entity);
            }

            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.CustomerMedicineHistoryRepository.GetByIdForCustomer(entity.Id, customerId);

            return new AddCustomerMedicineHistoryResult { History = saved!.ToDto(), WasUpdated = wasUpdated };
        }

        public async Task<CustomerMedicineHistoryDto?> ToggleMedicineActive(Guid customerId, Guid historyId)
        {
            var entity = await _unitOfWork.CustomerMedicineHistoryRepository.GetByIdForCustomer(historyId, customerId);
            if (entity is null)
            {
                return null;
            }

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return entity.ToDto();
        }

        public async Task<bool> DeleteMedicineHistory(Guid customerId, Guid historyId)
        {
            var entity = await _unitOfWork.CustomerMedicineHistoryRepository.GetByIdForCustomer(historyId, customerId);
            if (entity is null)
            {
                return false;
            }

            _unitOfWork.CustomerMedicineHistoryRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // ---------------- Allergies ----------------

        public async Task<IEnumerable<CustomerAllergyDto>?> GetAllergies(Guid customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer is null) return null;

            var links = await _unitOfWork.CustomerAllergyRepository.GetForCustomer(customerId);
            return links.Select(l => new CustomerAllergyDto
            {
                AllergyId = l.AllergyId,
                NameEn = l.Allergy.NameEn,
                NameAr = l.Allergy.NameAr,
            });
        }

        public async Task<AssignResult> AssignAllergy(Guid customerId, Guid allergyId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer is null) return new AssignResult { CustomerNotFound = true };

            var allergy = await _unitOfWork.AllergyRepository.GetById(allergyId);
            if (allergy is null) return new AssignResult { ReferenceNotFound = true };

            if (await _unitOfWork.CustomerAllergyRepository.Find(customerId, allergyId) is not null)
            {
                return new AssignResult { AlreadyAssigned = true };
            }

            _unitOfWork.CustomerAllergyRepository.Add(new CustomerAllergy { CustomerId = customerId, AllergyId = allergyId });
            await _unitOfWork.SaveAsync();

            return new AssignResult();
        }

        public async Task<bool> RemoveAllergy(Guid customerId, Guid allergyId)
        {
            var link = await _unitOfWork.CustomerAllergyRepository.Find(customerId, allergyId);
            if (link is null) return false;

            _unitOfWork.CustomerAllergyRepository.Remove(link);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // ---------------- Chronic conditions ----------------

        public async Task<IEnumerable<CustomerChronicConditionDto>?> GetChronicConditions(Guid customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer is null) return null;

            var links = await _unitOfWork.CustomerChronicConditionRepository.GetForCustomer(customerId);
            return links.Select(l => new CustomerChronicConditionDto
            {
                ChronicConditionId = l.ChronicConditionId,
                NameEn = l.ChronicCondition.NameEn,
                NameAr = l.ChronicCondition.NameAr,
            });
        }

        public async Task<AssignResult> AssignChronicCondition(Guid customerId, Guid chronicConditionId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer is null) return new AssignResult { CustomerNotFound = true };

            var condition = await _unitOfWork.ChronicConditionRepository.GetById(chronicConditionId);
            if (condition is null) return new AssignResult { ReferenceNotFound = true };

            if (await _unitOfWork.CustomerChronicConditionRepository.Find(customerId, chronicConditionId) is not null)
            {
                return new AssignResult { AlreadyAssigned = true };
            }

            _unitOfWork.CustomerChronicConditionRepository.Add(new CustomerChronicCondition
            {
                CustomerId = customerId,
                ChronicConditionId = chronicConditionId,
            });
            await _unitOfWork.SaveAsync();

            return new AssignResult();
        }

        public async Task<bool> RemoveChronicCondition(Guid customerId, Guid chronicConditionId)
        {
            var link = await _unitOfWork.CustomerChronicConditionRepository.Find(customerId, chronicConditionId);
            if (link is null) return false;

            _unitOfWork.CustomerChronicConditionRepository.Remove(link);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // ---------------- Organ functions ----------------


        public async Task<IEnumerable<CustomerOrganFunctionDto>?> GetOrganFunctions(Guid customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer is null) return null;

            var entries = await _unitOfWork.CustomerOrganFunctionRepository.GetForCustomer(customerId);
            return entries.Select(e => e.ToDto());
        }

        // One record per organ per customer — recording a new impairment level for an
        // organ that already has one UPDATES it (this reflects current function, not history).
        public async Task<AssignOrganFunctionResult> AssignOrganFunction(Guid customerId, AssignOrganFunctionDto dto)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer is null) return new AssignOrganFunctionResult { CustomerNotFound = true };

            var organ = await _unitOfWork.OrganRepository.GetById(dto.OrganId);
            if (organ is null) return new AssignOrganFunctionResult { OrganNotFound = true };

            var level = await _unitOfWork.OrganImpairmentLevelRepository.GetById(dto.OrganImpairmentLevelId);
            if (level is null) return new AssignOrganFunctionResult { ImpairmentLevelNotFound = true };

            var existing = await _unitOfWork.CustomerOrganFunctionRepository.FindByOrgan(customerId, dto.OrganId);
            CustomerOrganFunction entity;
            if (existing is not null)
            {
                existing.OrganImpairmentLevelId = dto.OrganImpairmentLevelId;
                existing.RecordedAt = DateTime.UtcNow;
                entity = existing;
            }
            else
            {
                entity = new CustomerOrganFunction
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    OrganId = dto.OrganId,
                    OrganImpairmentLevelId = dto.OrganImpairmentLevelId,
                    RecordedAt = DateTime.UtcNow,
                };
                _unitOfWork.CustomerOrganFunctionRepository.Add(entity);
            }

            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.CustomerOrganFunctionRepository.GetById(entity.Id);
            return new AssignOrganFunctionResult { OrganFunction = saved!.ToDto() };
        }

        public async Task<bool> RemoveOrganFunction(Guid customerId, Guid organFunctionId)
        {
            var entity = await _unitOfWork.CustomerOrganFunctionRepository.GetById(organFunctionId);
            if (entity is null || entity.CustomerId != customerId) return false;

            _unitOfWork.CustomerOrganFunctionRepository.Remove(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}