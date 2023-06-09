using Libro.Domain.Entities;

namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        User GetById(int userId);
        User GetByUsername(string username);
        void Add(User user);
        void Update(User user);
    }
}
