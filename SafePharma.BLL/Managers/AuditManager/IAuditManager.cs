using SafePharma.BLL.DTOs.Audit;
using SafePharma.Common;
using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IAuditManager
    {
        Task<GeneralResult<IEnumerable<AuditReadDto>>> GetAllAudit();
        Task<GeneralResult<AuditCreateDto>> CreateAudit(object newValues,object oldValues, ActionsEnum actionsEnum);
        Task<GeneralResult<IEnumerable<AuditReadDto>>> GetRecentActivity(int take = 6);

    }
}