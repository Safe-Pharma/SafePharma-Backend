using SafePharma.Common.Enums;

namespace SafePharma.DAL
{
    public class Notification : IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid PharmacyId { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;

        public string MessageEn { get; set; } = string.Empty;
        public string MessageAr { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public NotificationPriority Priority { get; set; }
        public Guid? ReferenceId { get; set; }
        public NotificationReferenceType? ReferenceType { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
