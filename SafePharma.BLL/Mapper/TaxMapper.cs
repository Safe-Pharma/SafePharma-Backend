using SafePharma.DAL;

namespace SafePharma.BLL
{
    public static class TaxMapper
    {
        public static TaxDto ToDto(this Tax entity)
        {
            return new TaxDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Rate = entity.Rate,
                Status = entity.Status.ToString()
            };
        }

        public static Tax ToEntity(this TaxCreateDto dto)
        {
            return new Tax
            {
                Name = dto.Name,
                Rate = dto.Rate,
                Status = ParseStatus(dto.Status)
            };
        }

        public static void ApplyTo(this TaxUpdateDto dto, Tax entity)
        {
            entity.Name = dto.Name;
            entity.Rate = dto.Rate;
            entity.Status = ParseStatus(dto.Status);
        }

        private static TaxStatus ParseStatus(string status)
        {
            return Enum.Parse<TaxStatus>(status, ignoreCase: true);
        }
    }
}
