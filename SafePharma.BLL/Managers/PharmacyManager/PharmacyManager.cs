using SafePharma.BLL.DTOs.PharmacyDtos;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL.Managers.PharmacyManager
{
    public class PharmacyManager : IPharmacyManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public PharmacyManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GeneralResult<IEnumerable<PharmacyReadDto>>> GetAllPharmacies()
        {
             var pharmacies = await _unitOfWork.PharmacyRepository.GetAll();

            if (!pharmacies.Any())
                return GeneralResult<IEnumerable<PharmacyReadDto>>.NotFound("No pharmacies found");

            IEnumerable<PharmacyReadDto> res = pharmacies.Select(p => new PharmacyReadDto
            {
                Id = p.Id,
                Name = p.Name,
                CommercialRegistration = p.CommercialRegistration,
                Address = p.Address,
                Country = p.Country,
                City = p.City,
                Phone = p.Phone,
                BusinessEmail = p.BusinessEmail,
                IsActive = p.IsActive,
                SubscriptionId = p.SubscriptionId,
            }).ToList();
            return GeneralResult<IEnumerable<PharmacyReadDto>>.SuccessResult(res!);
        }

        public async Task<GeneralResult> UpdatePharmacyStatus(Guid id)
        {
            var pharmacy = await _unitOfWork.PharmacyRepository.GetById(id);
            if (pharmacy == null)
            {
                return GeneralResult.NotFound("This pharmacy not found");
            }
            pharmacy.IsActive = !pharmacy.IsActive;
            pharmacy.UpdatedAt = DateTime.UtcNow;  

            await _unitOfWork.SaveAsync();
            return GeneralResult.SuccessResult();
        }
    }
}
