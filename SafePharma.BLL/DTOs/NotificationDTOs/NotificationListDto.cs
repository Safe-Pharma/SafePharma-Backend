using SafePharma.Common.Enums;

namespace SafePharma.BLL
{
    public class NotificationListDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = default!;

        public string Message { get; set; } = default!;

        public NotificationType Type { get; set; }

        public NotificationPriority Priority { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
