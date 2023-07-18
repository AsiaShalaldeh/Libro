using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Libro.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    //[Authorize(AuthenticationSchemes = "Bearer", Roles = "Librarian")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Send overdue notifications for books.
        /// </summary>
        /// <returns>A response indicating the success of the operation.</returns>
        [HttpPost("overdue-books/send")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendOverdueNotification()
        {
            try
            {
                var isSent = await _notificationService.SendOverdueNotification();
                if (isSent)
                {
                    _logger.LogInformation("Overdue notification sent successfully");
                    return Ok("Overdue notification sent successfully");
                }
                else
                {
                    _logger.LogInformation("No Overdue Books Found");
                    return NotFound("No Overdue Books Found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending overdue notification");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Send a reminder notification.
        /// </summary>
        /// <param name="request">The reminder notification request data.</param>
        /// <returns>A response indicating the success of the operation.</returns>
        [HttpPost("reminder/send")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendReminderNotification([FromBody] ReminderNotificationModel request)
        {
            try
            {
                var recipientEmail = request.RecipientEmail;
                var bookISBN = request.BookISBN;
                var recipientId = request.RecipientId;

                // Send the reminder notification
                var success = await _notificationService.SendReminderNotification(recipientEmail, bookISBN, recipientId);

                if (success)
                {
                    _logger.LogInformation($"Reminder notification sent to {recipientEmail} for book with ISBN = '{bookISBN}'.");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation($"No overdue transaction found for {recipientEmail}.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the reminder notification.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while sending the reminder notification.");
            }
        }
    }
}