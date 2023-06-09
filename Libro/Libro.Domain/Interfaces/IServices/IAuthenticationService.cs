using Libro.Domain.Enums;

namespace Libro.Domain.Interfaces.IServices
{
    public interface IAuthenticationService
    {
        bool Register(string username, string password);
        string Login(string username, string password);
        void AssignRole(int userId, string role);
    }
}
