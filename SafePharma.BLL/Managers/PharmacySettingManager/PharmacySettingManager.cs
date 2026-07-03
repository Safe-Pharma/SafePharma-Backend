using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class PharmacySettingManager : IPharmacySettingManager
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IMapper _mapper;

        public PharmacySettingManager(IUnitOfWork unitOfWork
            //IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            //_mapper = mapper;
        }

        public async Task<PharmacySettings?> GetSettings()
        {
            return await _unitOfWork.PharmacySettingRepository.GetSettings();
        }

        public async Task<PharmacySettingsUpdateDto> updatePharamcySettings(PharmacySettingsUpdateDto dto)
        {
            var entity = await _unitOfWork.PharmacySettingRepository.GetSettings();

            //_mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveAsync();

            return dto;
        }
    }
}
