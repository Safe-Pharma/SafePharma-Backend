using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class CustomerRelativesManager : ICustomerRelativesManager
    {
        public IUnitOfWork _unitOfWork;

        public CustomerRelativesManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GeneralResult<IEnumerable<CustomerRelativeReadDto>>> GetRelations(Guid id)
        {

            var customer = await _unitOfWork.CustomerRepository.GetByIdWithRealtives(id);
            if (customer is null)
            {
                return GeneralResult<IEnumerable<CustomerRelativeReadDto>>.NotFound();
            }

            var CustomerRels = customer!.Relatives;

            var custRels = CustomerRels.Select(
                a =>
                new CustomerRelativeReadDto
                {
                    RelativeId = a.RelativeId,
                    RelativeName = a.Relative.Name,
                    RelativePhone = a.Relative.Phone,
                }
                );

            return GeneralResult<IEnumerable<CustomerRelativeReadDto>>.SuccessResult(custRels);
        }

        public async Task<GeneralResult> CreateRelation(CustomerRelativeCreateDto dto)
        {
            if (dto is null)
            {
                GeneralResult<CustomerRelative>.NotFound();
            }
            Customer customer = await _unitOfWork.CustomerRepository.GetById(dto!.CustomerId);
            Customer relative = await _unitOfWork.CustomerRepository.GetById(dto.RelativeId);
            if (customer is null || relative is null)
            {
                GeneralResult<CustomerRelative>.NotFound();

            }

            var custRel = new CustomerRelative
            {
                CustomerId = dto.CustomerId,
                RelativeId = dto.RelativeId,
                Customer = customer!,
                Relative = relative!

            };
            _unitOfWork._customerRelativesRepository.Add(custRel);
            await _unitOfWork.SaveAsync();

            return GeneralResult.SuccessResult();
        }

    }
}
