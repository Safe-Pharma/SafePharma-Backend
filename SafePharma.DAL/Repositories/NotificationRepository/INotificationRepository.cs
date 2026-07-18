using SafePharma.Common.Enums;

namespace SafePharma.DAL
{
    public interface INotificationRepository : IGenircRepository<Notification>
    {
        Task<bool> ExistsAsync(Guid pharmacyId, NotificationType type, Guid referenceId);
        Task<int> GetUnreadCountAsync(Guid pharmacyId);
        Task<List<Notification>> GetUnreadAsync(Guid pharmacyId);



    }
}
