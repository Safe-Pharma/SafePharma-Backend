using SafePharma.BLL;
using SafePharma.BLL.DTOs.NotificationDTOs;
using SafePharma.Common;

public interface INotificationManager
{
    Task<GeneralResult<IEnumerable<NotificationListDto>>> GetAll();

    Task<GeneralResult<NotificationCountDto>> GetUnreadCount();

    Task<GeneralResult<bool>> MarkAsRead(Guid notificationId);

    Task<GeneralResult<bool>> MarkAllAsRead();

    Task<GeneralResult<bool>> CreateBatchExpiryNotification(
        Guid pharmacyId,
        Guid batchId,
        string medicineNameEn,
        string medicineNameAr,
        string batchNumber,
        int daysRemaining);

    Task<GeneralResult<bool>> CreateLowStockNotification(
        Guid pharmacyId,
        Guid medicineId,
        string medicineNameEn,
        string medicineNameAr,
        int currentQuantity,
        int minimumQuantity);
}