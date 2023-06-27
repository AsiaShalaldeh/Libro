using Libro.Domain.Dtos;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IServices;
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

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("overdue/send")]
        public async Task<IActionResult> SendOverdueNotification()
        {
            try
            {
                var isSent = await _notificationService.SendOverdueNotification();
                if (isSent)
                    return Ok("Overdue notification sent successfully");
                else
                    return NotFound("No Overdue Books Found");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost("reservation/send")]
        public async Task<IActionResult> SendReservationNotification([FromBody] ReservationNotificationRequest request)
        {
            try
            {
                var response = await _notificationService.SendReservationNotification(request.RecipientEmail
                    , request.BookTitle, request.RecipientId);
                if (response)
                {
                    return Ok("Reservation notification sent successfully");
                }
                else
                {
                    return NotFound($"No Reservation done by Patron ID = {request.RecipientId} Recently !!");
                }
            }
            catch(ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }

}
