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
            if (pharmacies == null)
            {
                return GeneralResult<IEnumerable<PharmacyReadDto>>.NotFound();
            }
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
                IsActive = p.isActive,
                SubscriptionId = p.SubscriptionId,
            }).ToList();
            return GeneralResult<IEnumerable<PharmacyReadDto>>.SuccessResult(res!);
        }

        public async Task<GeneralResult> UpdatePharmacyStatus(Guid id)
        {
            var pharmacy = await _unitOfWork.PharmacyRepository.GetById(id);
            if (pharmacy == null)
            {
                return GeneralResult.NotFound("This pharmacy not foung");
            }
            pharmacy.isActive = !pharmacy.isActive;
            _unitOfWork.SaveAsync();
            return GeneralResult.SuccessResult();
        }
    }
}
