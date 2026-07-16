using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class CustomerManager : ICustomerManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomers(string? search = null)
        {
            var customers = await _unitOfWork.CustomerRepository.Search(search);
            return customers.Select(c => c.ToDto());
        }

        public async Task<CustomerDto?> GetCustomerById(Guid id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(id);
            return customer?.ToDto();
        }

        public async Task<CustomerStatsDto> GetStats()
        {
            var customers = (await _unitOfWork.CustomerRepository.GetAll()).ToList();

            var active = customers.Count(c => c.Status == CustomerStatus.Active);

            return new CustomerStatsDto
            {
                TotalCustomers = customers.Count,
                Active = active,
                Inactive = customers.Count - active,
                TotalPaidAllCustomers = customers.Sum(c => c.TotalPaid)
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

        public async Task<CustomerDto?> ToggleStatus(Guid id)
        {
            var entity = await _unitOfWork.CustomerRepository.GetById(id);
            if (entity is null)
            {
                return null;
            }

            entity.Status = entity.Status == CustomerStatus.Active ? CustomerStatus.Inactive : CustomerStatus.Active;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return entity.ToDto();
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

            var entity = dto.ToEntity();
            entity.Id = Guid.NewGuid();
            entity.CustomerId = customerId;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.CustomerMedicineHistoryRepository.Add(entity);
            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.CustomerMedicineHistoryRepository.GetByIdForCustomer(entity.Id, customerId);

            return new AddCustomerMedicineHistoryResult { History = saved!.ToDto() };
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
    }
}
