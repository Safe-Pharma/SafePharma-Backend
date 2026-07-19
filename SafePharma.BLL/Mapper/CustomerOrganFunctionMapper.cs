using SafePharma.DAL;

namespace SafePharma.BLL
{
    public static class CustomerOrganFunctionMapper
    {
        public static CustomerOrganFunctionDto ToDto(this CustomerOrganFunction entity)
        {
            return new CustomerOrganFunctionDto
            {
                Id = entity.Id,
                OrganId = entity.OrganId,
                OrganNameEn = entity.Organ.NameEn,
                OrganNameAr = entity.Organ.NameAr,
                OrganImpairmentLevelId = entity.OrganImpairmentLevelId,
                ImpairmentLevelNameEn = entity.OrganImpairmentLevel.NameEn,
                ImpairmentLevelNameAr = entity.OrganImpairmentLevel.NameAr,
                RecordedAt = entity.RecordedAt,
            };
        }
    }
}