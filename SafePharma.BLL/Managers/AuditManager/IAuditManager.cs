using SafePharma.BLL;
using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IAuditManager
    {
        Task<GeneralResult<IEnumerable<AuditReadDto>>> GetAllAudit();
        Task<GeneralResult<bool>> CreateAudit(object newValues,object oldValues,string entity, ActionsEnum actionsEnum);
        Task<GeneralResult<IEnumerable<AuditReadDto>>> GetRecentActivity(int take = 6);

    }
}