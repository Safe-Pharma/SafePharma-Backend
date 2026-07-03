using SafePharma.BLL;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class AuditManager : IAuditManager
    {
        public IUnitOfWork _unitOfWork;

        public AuditManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<GeneralResult<IEnumerable<AuditReadDto>>> GetAllAudit()
        {
            var auditList = await _unitOfWork._auditRepository.GetAll();
            IEnumerable<AuditReadDto> auditReadList = auditList.Select(a => new AuditReadDto
            {
                Entity = a.Entity,
                Action = a.Action,
                Date = a.Date,
                Device = a.Device,
            }).ToList();
            return GeneralResult<IEnumerable<AuditReadDto>>.SuccessResult(auditReadList);
        }
    }
}
