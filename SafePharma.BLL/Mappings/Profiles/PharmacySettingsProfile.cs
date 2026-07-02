using AutoMapper;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class PharmacySettingsProfile : Profile
    {
        public PharmacySettingsProfile()
        {
            CreateMap<PharmacySettings, PharmacySettingsUpdateDto>();
            CreateMap<PharmacySettingsUpdateDto, PharmacySettings>();
        }
    }
}
