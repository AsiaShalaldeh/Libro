using Libro.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Libro.Infrastructure.Data
{
    public class LibroUser : IdentityUser
    {
        public UserRole Role { get; set; }
    }
}
