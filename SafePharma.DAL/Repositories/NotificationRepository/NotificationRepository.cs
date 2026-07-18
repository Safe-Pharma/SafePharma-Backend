using Microsoft.EntityFrameworkCore;
using SafePharma.Common.Enums;

namespace SafePharma.DAL
{
    public class NotificationRepository : GenircRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext db) : base(db)
        {
        }


        public async Task<bool> ExistsAsync(Guid pharmacyId ,NotificationType type ,Guid referenceId)
        {
            return await _db.Notifications.AnyAsync(x =>
                x.PharmacyId == pharmacyId &&
                x.Type == type &&
                x.ReferenceId == referenceId);
        }

        public async Task<int> GetUnreadCountAsync(Guid pharmacyId)
        {
            return await _db.Notifications.CountAsync(x =>
                    x.PharmacyId == pharmacyId &&
                    !x.IsRead);
        }

        public async Task<List<Notification>> GetUnreadAsync(Guid pharmacyId)
        {
            return await _db.Notifications.Where(x =>
                    x.PharmacyId == pharmacyId &&
                    !x.IsRead)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
