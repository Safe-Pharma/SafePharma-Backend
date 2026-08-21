using SafePharma.BLL.DTOs.NotificationDTOs;
using SafePharma.Common;
using SafePharma.Common.Enums;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class NotificationManager : INotificationManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserContext _currentUserContext;

        public NotificationManager(
            IUnitOfWork unitOfWork,
            ICurrentUserContext currentUserContext)
        {
            _unitOfWork = unitOfWork;
            _currentUserContext = currentUserContext;
        }

        #region Public Methods

        public async Task<GeneralResult<IEnumerable<NotificationListDto>>> GetAll()
        {
            var notifications = await _unitOfWork.Notifications
                .GetAll(_currentUserContext.PharmacyId);

            var result = notifications.Select(n => new NotificationListDto
            {
                Id = n.Id,

                TitleEn = n.TitleEn,
                TitleAr = n.TitleAr,

                MessageEn = n.MessageEn,
                MessageAr = n.MessageAr,

                Type = n.Type,
                Priority = n.Priority,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });

            return GeneralResult<IEnumerable<NotificationListDto>>
                .SuccessResult(result);
        }

        public async Task<GeneralResult<NotificationCountDto>> GetUnreadCount()
        {
            var count = await _unitOfWork.Notifications
                .GetUnreadCountAsync(_currentUserContext.PharmacyId);

            return GeneralResult<NotificationCountDto>.SuccessResult(
                new NotificationCountDto
                {
                    Count = count
                });
        }

        public async Task<GeneralResult<bool>> MarkAsRead(Guid notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetById(notificationId);

            if (notification is null ||
                notification.PharmacyId != _currentUserContext.PharmacyId)
            {
                return GeneralResult<bool>.FailResult("Notification not found.");
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return GeneralResult<bool>.SuccessResult(true);
        }

        public async Task<GeneralResult<bool>> MarkAllAsRead()
        {
            var notifications = await _unitOfWork.Notifications
                .GetAll(_currentUserContext.PharmacyId);

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveAsync();

            return GeneralResult<bool>.SuccessResult(true);
        }

        public async Task<GeneralResult<bool>> CreateBatchExpiryNotification(
        Guid pharmacyId,
        Guid batchId,
        string medicineNameEn,
        string medicineNameAr,
        string batchNumber,
        int daysRemaining)
        {
            NotificationType type;
            NotificationPriority priority;

            if (daysRemaining <= 0)
            {
                type = NotificationType.BatchExpired;
                priority = NotificationPriority.Critical;
            }
            else if (daysRemaining <= 30)
            {
                type = NotificationType.BatchExpiry30;
                priority = NotificationPriority.High;
            }
            else if (daysRemaining <= 60)
            {
                type = NotificationType.BatchExpiry60;
                priority = NotificationPriority.Medium;
            }
            else
            {
                type = NotificationType.BatchExpiry90;
                priority = NotificationPriority.Low;
            }

            return await CreateNotification(
                pharmacyId,
                type,
                priority,
                batchId,
                NotificationReferenceType.Batch,

                $"Batch Expiry ({daysRemaining} Days)",
                $"انتهاء صلاحية الدفعة ({daysRemaining} يوم)",

                $"{medicineNameEn} (Batch {batchNumber}) expires in {daysRemaining} day(s).",
                $"{medicineNameAr} (الدفعة {batchNumber}) ستنتهي صلاحيتها خلال {daysRemaining} يوم.");
        }

        public async Task<GeneralResult<bool>> CreateLowStockNotification(
        Guid pharmacyId,
        Guid medicineId,
        string medicineNameEn,
        string medicineNameAr,
        int currentQuantity,
        int minimumQuantity)
        {
            return await CreateNotification(
                pharmacyId,
                NotificationType.LowStock,
                NotificationPriority.High,
                medicineId,
                NotificationReferenceType.Medicine,

                "Low Stock",
                "مخزون منخفض",

                $"{medicineNameEn} stock is low. Current quantity: {currentQuantity}, Minimum quantity: {minimumQuantity}.",
                $"مخزون {medicineNameAr} منخفض. الكمية الحالية: {currentQuantity}، والحد الأدنى: {minimumQuantity}.");
        }

        #endregion

        #region Private Methods

        private async Task<GeneralResult<bool>> CreateNotification(
        Guid pharmacyId,
        NotificationType type,
        NotificationPriority priority,
        Guid referenceId,
        NotificationReferenceType referenceType,
        string titleEn,
        string titleAr,
        string messageEn,
        string messageAr)
        {
            var exists = await _unitOfWork.Notifications.ExistsAsync(
                pharmacyId,
                type,
                referenceId);

            if (exists)
                return GeneralResult<bool>.SuccessResult(true);

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                PharmacyId = pharmacyId,

                TitleEn = titleEn,
                TitleAr = titleAr,

                MessageEn = messageEn,
                MessageAr = messageAr,

                Type = type,
                Priority = priority,
                ReferenceId = referenceId,
                ReferenceType = referenceType,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.Notifications.Add(notification);

            await _unitOfWork.SaveAsync();

            return GeneralResult<bool>.SuccessResult(true);
        }

        #endregion
    }
}
