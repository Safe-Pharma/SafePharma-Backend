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
            var CustomerRelsTo = customer!.RelatedTo;


            var custRels = CustomerRels
                        .Select(a => new CustomerRelativeReadDto
                        {
                            RelativeId = a.RelativeId,
                            RelativeName = a.Relative.Name,
                            RelativePhone = a.Relative.Phone,
                        })
                        .Concat(
                            CustomerRelsTo.Select(a => new CustomerRelativeReadDto
                            {
                                RelativeId = a.CustomerId,
                                RelativeName = a.Customer.Name,
                                RelativePhone = a.Customer.Phone,
                            })
                        );


            return GeneralResult<IEnumerable<CustomerRelativeReadDto>>.SuccessResult(custRels);
        }

        public async Task<GeneralResult<IEnumerable<CustomerRelativeReadDto>>> GetChilds(Guid id)
        {

            var customer = await _unitOfWork.CustomerRepository.GetByIdWithChilds(id);
            if (customer is null)
            {
                return GeneralResult<IEnumerable<CustomerRelativeReadDto>>.NotFound();
            }

            var CustomerRels = customer!.Relatives;


            var custRels = CustomerRels
                        .Select(a => new CustomerRelativeReadDto
                        {
                            RelativeId = a.RelativeId,
                            RelativeName = a.Relative.Name,
                            RelativePhone = a.Relative.Phone,
                        })
                       ;


            return GeneralResult<IEnumerable<CustomerRelativeReadDto>>.SuccessResult(custRels);
        }

        public async Task<GeneralResult> CreateRelation(CustomerRelativeCreateDto dto)
        {
            if (dto is null)
            {
                return GeneralResult<CustomerRelative>.NotFound();
            }
            Customer customer = await _unitOfWork.CustomerRepository.GetById(dto!.CustomerId);
            Customer relative = await _unitOfWork.CustomerRepository.GetById(dto.RelativeId);
            if (customer is null || relative is null)
            {
                return GeneralResult<CustomerRelative>.NotFound();

            }

            var custRel = new CustomerRelative
            {
                CustomerId = dto.CustomerId,
                RelativeId = dto.RelativeId,
                Customer = customer!,
                Relative = relative!,
                HasAccessToRelative = dto.HasAccessToRelative
            };

            _unitOfWork._customerRelativesRepository.Add(custRel);
            await _unitOfWork.SaveAsync();

            return GeneralResult.SuccessResult();
        }

        public async Task<GeneralResult> RemoveRelation(Guid id)
        {
            var link = await _unitOfWork._customerRelativesRepository.GetById(id);
            if (link is null)
            {
                return GeneralResult.FailResult("Relative link not found.");
            }

            _unitOfWork._customerRelativesRepository.Delete(link);
            await _unitOfWork.SaveAsync();

            return GeneralResult.SuccessResult();
        }

        public async Task<bool> CanAccessAsync(Guid requesterId, Guid targetCustomerId)
        {
             if (requesterId == targetCustomerId)
            {
                return true;
            }

             if (requesterId == Guid.Empty || targetCustomerId == Guid.Empty)
            {
                
                return false;
            }

             var hasAccess = await _unitOfWork._customerRelativesRepository
                .HasPortalAccessAsync(requesterId, targetCustomerId);

            

            return hasAccess;
        }

    }
}
