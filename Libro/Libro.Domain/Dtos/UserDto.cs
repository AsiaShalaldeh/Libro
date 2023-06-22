using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Dtos
{
    public class UserDto
    {
        [Required]
        public string UserName { get; set; } = String.Empty;

        [Required]
        public string Password { get; set; } = String.Empty;

    }
}
