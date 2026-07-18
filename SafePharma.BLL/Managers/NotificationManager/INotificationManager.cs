using SafePharma.BLL.DTOs.NotificationDTOs;
using SafePharma.Common;

namespace SafePharma.BLL
{
    internal interface INotificationManager
    {
        Task<GeneralResult<IEnumerable<NotificationListDto>>> GetAll();
        Task<GeneralResult<NotificationCountDto>> GetUnreadCount();

        Task<GeneralResult<bool>> MarkAsRead(Guid notificationId);

        Task<GeneralResult<bool>> MarkAllAsRead();

        Task<GeneralResult<bool>> CreateBatchExpiryNotification(Guid pharmacyId 
            ,Guid batchId 
            ,string medicineName 
            ,string batchNumber,
            int daysRemaining);

        Task<GeneralResult<bool>> CreateLowStockNotification(
            Guid pharmacyId,
            Guid medicineId,
            string medicineName,
            int currentQuantity,
            int minimumQuantity);
    }
}
