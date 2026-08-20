using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.BLL.DTOs.NotificationDTOs;
using SafePharma.Common;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationManager _notificationManager;

        public NotificationController(
            INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        /// <summary>
        /// Get all notifications for the current pharmacy.
        /// PharmacyId is automatically taken from the JWT token.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<GeneralResult<IEnumerable<NotificationListDto>>>> GetAll()
        {
            var result = await _notificationManager.GetAll();

            return Ok(result);
        }

        /// <summary>
        /// Get unread notifications count.
        /// Useful for notification badge in Angular.
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<GeneralResult<NotificationCountDto>>> GetUnreadCount()
        {
            var result = await _notificationManager.GetUnreadCount();

            return Ok(result);
        }

        /// <summary>
        /// Mark one notification as read.
        /// </summary>
        [HttpPatch("{notificationId:guid}/read")]
        public async Task<ActionResult<GeneralResult<bool>>> MarkAsRead(
            Guid notificationId)
        {
            var result = await _notificationManager.MarkAsRead(notificationId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Mark all notifications for the current pharmacy as read.
        /// </summary>
        [HttpPatch("read-all")]
        public async Task<ActionResult<GeneralResult<bool>>> MarkAllAsRead()
        {
            var result = await _notificationManager.MarkAllAsRead();

            return Ok(result);
        }
    }
}
