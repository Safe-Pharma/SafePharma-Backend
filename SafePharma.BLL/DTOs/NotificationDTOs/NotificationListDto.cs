using SafePharma.Common.Enums;

namespace SafePharma.BLL
{
    public class NotificationListDto
    {
        public Guid Id { get; set; }

        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;

        public string MessageEn { get; set; } = string.Empty;
        public string MessageAr { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public NotificationPriority Priority { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
