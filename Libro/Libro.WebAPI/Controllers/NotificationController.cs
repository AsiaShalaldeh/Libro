using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace Libro.WebAPI.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Librarian")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpPost("overdue/send")]
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

        [HttpPost("reservation/send")]
        public async Task<IActionResult> SendReservationNotification([FromBody] ReservationNotificationRequest request)
        {
            try
            {
                var response = await _notificationService.SendReservationNotification(request.RecipientEmail, request.BookTitle, request.RecipientId);
                if (response)
                {
                    _logger.LogInformation("Reservation notification sent successfully");
                    return Ok("Reservation notification sent successfully");
                }
                else
                {
                    _logger.LogInformation($"No Reservation done by Patron ID = {request.RecipientId} Recently for {request.BookTitle} Book !!");
                    return NotFound($"No Reservation done by Patron ID = {request.RecipientId} Recently for {request.BookTitle} Book !!");
                }
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, $"Resource not found: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending reservation notification");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
