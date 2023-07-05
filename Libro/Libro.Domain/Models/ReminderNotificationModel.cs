using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Models
{
    public class ReminderNotificationModel
    {
        [Required(ErrorMessage = "RecipientId is required")]
        public string RecipientId { get; set; }

        [Required(ErrorMessage = "RecipientEmail is required")]
        [EmailAddress(ErrorMessage = "RecipientEmail is not a valid email address")]
        public string RecipientEmail { get; set; }

        [Required(ErrorMessage = "BookISBN is required")]
        public string BookISBN { get; set; }
    }
}
