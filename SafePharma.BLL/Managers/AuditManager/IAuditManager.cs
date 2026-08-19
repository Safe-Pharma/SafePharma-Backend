using SafePharma.BLL.DTOs.Audit;
using SafePharma.Common;
using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IAuditManager
    {
        Task<GeneralResult<IEnumerable<AuditReadDto>>> GetAllAudit(Guid pharmacyId);
        Task<GeneralResult<AuditCreateDto>> CreateAudit(object newValues,object oldValues, ActionsEnum actionsEnum);

    }
}