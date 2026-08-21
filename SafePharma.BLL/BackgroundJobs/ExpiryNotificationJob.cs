using SafePharma.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.BLL.BackgroundJobs
{
    public class ExpiryNotificationJob : IExpiryNotificationJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationManager _notificationManager;

        public ExpiryNotificationJob(
            IUnitOfWork unitOfWork,
            INotificationManager notificationManager)
        {
            _unitOfWork = unitOfWork;
            _notificationManager = notificationManager;
        }

        public async Task Execute()
        {
            var batches = await _unitOfWork
                ._batchRepository
                .GetBatchesForExpiryNotifications();

            var today = DateTime.UtcNow.Date;

            foreach (var batch in batches)
            {
                var daysRemaining = (batch.ExpiryDate.Date - today).Days;

                
                if (daysRemaining > 90)
                    continue;

                await _notificationManager.CreateBatchExpiryNotification(
                    batch.PharmacyId,
                    batch.Id,
                    batch.Medicine.TradeNameEn,
                    batch.Medicine.TradeNameAr,
                    batch.BatchNumber,
                    daysRemaining);
            }
        }
    }
}
