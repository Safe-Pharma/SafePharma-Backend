using SafePharma.DAL;

namespace SafePharma.BLL
{
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(this Customer entity, decimal totalPaid = 0m)
        {
            return new CustomerDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Phone = entity.Phone,
                Email = entity.Email ?? string.Empty,
                Address = entity.Address ?? string.Empty,
                DateOfBirth = entity.DateOfBirth,
                Notes = entity.Notes ?? string.Empty,
                Status = entity.Status.ToString(),
                TotalPaid = totalPaid,
            };
        }

        public static Customer ToEntity(this CustomerCreateDto dto)
        {
            return new Customer
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                Notes = dto.Notes,
                Status = ParseStatus(dto.Status),
            };
        }

        public static void ApplyTo(this CustomerUpdateDto dto, Customer entity)
        {
            entity.Name = dto.Name;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.Address = dto.Address;
            entity.DateOfBirth = dto.DateOfBirth;
            entity.Notes = dto.Notes;
            entity.Status = ParseStatus(dto.Status);
        }
        public static void ApplyToFromPortal(this CustomerUpdatePortalDto dto, Customer entity)
        {
            entity.Name = dto.Name;
            entity.Email = dto.Email;
            entity.Address = dto.Address;
            entity.DateOfBirth = dto.DateOfBirth;
            entity.Notes = dto.Notes;
        }
        private static CustomerStatus ParseStatus(string status)
        {
            return Enum.Parse<CustomerStatus>(status, ignoreCase: true);
        }
    }
}