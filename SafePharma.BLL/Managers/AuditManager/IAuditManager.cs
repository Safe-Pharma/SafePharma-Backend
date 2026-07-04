using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IAuditManager
    {
        Task<GeneralResult<IEnumerable<AuditReadDto>>> GetAllAudit();
    }
}