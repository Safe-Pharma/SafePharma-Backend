using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class CustomerRelativesManager : ICustomerRelativesManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserContext _currentUserContext;

        public CustomerRelativesManager(IUnitOfWork unitOfWork, ICurrentUserContext currentUserContext)
        {
            _unitOfWork = unitOfWork;
            _currentUserContext = currentUserContext;
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
                            Id=a.Id,
                            RelativeId = a.RelativeId,
                            RelativeName = a.Relative.Name,
                            RelativePhone = a.Relative.Phone??"",
                        })
                        .Concat(
                            CustomerRelsTo.Select(a => new CustomerRelativeReadDto
                            {
                                Id = a.Id,
                                RelativeId = a.CustomerId,
                                RelativeName = a.Customer.Name,
                                RelativePhone = a.Customer.Phone??"",
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
                           Id = a.Id,
                           RelativeId = a.RelativeId,
                           RelativeName = a.Relative.Name,
                           RelativePhone = a.Relative.Phone ?? "",
                       });


            return GeneralResult<IEnumerable<CustomerRelativeReadDto>>.SuccessResult(custRels);
        }

        public async Task<GeneralResult> CreateRelation(CustomerRelativeCreateDto dto)
        {
             if (dto is null)
                return GeneralResult.NotFound("Relation data is required");

             if (dto.CustomerId == dto.RelativeId)
                return GeneralResult.NotFound("A customer cannot be their own relative");

              
            var customer = await _unitOfWork.CustomerRepository.GetById(dto.CustomerId);
            if (customer is null)
                return GeneralResult.NotFound($"Customer not found");

             var relative = await _unitOfWork.CustomerRepository.GetById(dto.RelativeId);
            if (relative is null)
                return GeneralResult.NotFound($"Relative not found");

            var alreadyExists = await _unitOfWork._customerRelativesRepository
               .IsFound(dto.CustomerId,dto.RelativeId);
            if (alreadyExists)
                return GeneralResult.NotFound("This relationship already exists");

            var custRel = new CustomerRelative
            {
                CustomerId = dto.CustomerId,
                RelativeId = dto.RelativeId,
                HasAccessToRelative = dto.HasAccessToRelative,
                CreatedAt=DateTime.UtcNow
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
            
            if (requesterId == Guid.Empty || targetCustomerId == Guid.Empty)
                return false;

            
            if (requesterId == targetCustomerId)
                return true;

             
            return await _unitOfWork._customerRelativesRepository
                .HasPortalAccessAsync(requesterId, targetCustomerId);
        }

    }
}
