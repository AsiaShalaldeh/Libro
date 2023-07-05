using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Libro.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpPost("overdue-books/send")]
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

        [HttpPost("reminder/send")]
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
                return StatusCode(500, "An error occurred while sending the reminder notification.");
            }
        }
    }
}
