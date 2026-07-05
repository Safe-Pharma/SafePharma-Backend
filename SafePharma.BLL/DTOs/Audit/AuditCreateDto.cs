using SafePharma.Common.Enums;

namespace SafePharma.BLL.DTOs.Audit
{
    public class AuditCreateDto
    {
        public DateTime Date { get; set; }
        public ActionsEnum Action { get; set; }
        public string Entity { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public Guid UserId { get; set; }

        public string newValues { get; set; }
        public string oldValues { get; set; }

    }
}
