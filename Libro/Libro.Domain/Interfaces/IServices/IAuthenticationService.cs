using Libro.Domain.Enums;
using Libro.Domain.Models;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IAuthenticationService
    {
        Task<Response> Register(string username, string email, string password);
        Task<string> Login(string username, string password);
        Task<Response> AssignRole(string userId, string role);
    }
}
