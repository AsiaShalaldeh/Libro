using Libro.Domain.Enums;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IAuthenticationService
    {
        Task<bool> Register(string username, string email, string password);
        Task<string> Login(string username, string password);
        Task<bool> AssignRole(string userId, string role);
    }
}
