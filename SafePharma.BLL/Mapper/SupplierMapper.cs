using SafePharma.DAL;

namespace SafePharma.BLL
{
    public static class SupplierMapper
    {
        public static SupplierDto ToDto(this Supplier entity)
        {
            return new SupplierDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactPerson = entity.ContactPerson,
                Phone = entity.Phone,
                Email = entity.Email,
                TaxNumber = entity.TaxNumber ?? string.Empty,
                Address = entity.Address,
                Country = entity.Country?.Name ?? string.Empty,
                Status = entity.Status.ToString(),
                Outstanding = entity.Outstanding,
            };
        }

        public static Supplier ToEntity(this SupplierCreateDto dto)
        {
            return new Supplier
            {
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email,
                TaxNumber = dto.TaxNumber,
                Address = dto.Address,
                CountryId = dto.CountryId,
                Status = ParseStatus(dto.Status),
                Outstanding = dto.Outstanding,
            };
        }

        public static void ApplyTo(this SupplierUpdateDto dto, Supplier entity)
        {
            entity.Name = dto.Name;
            entity.ContactPerson = dto.ContactPerson;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.TaxNumber = dto.TaxNumber;
            entity.Address = dto.Address;
            entity.CountryId = dto.CountryId;
            entity.Status = ParseStatus(dto.Status);
            entity.Outstanding = dto.Outstanding;
        }

        private static SupplierStatus ParseStatus(string status)
        {
            return Enum.Parse<SupplierStatus>(status, ignoreCase: true);
        }
    }
}
