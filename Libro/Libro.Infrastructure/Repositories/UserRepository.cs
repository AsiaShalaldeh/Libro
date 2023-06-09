using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users; // will be replaced by _context

        public UserRepository()
        {
            _users = new List<User>()
            {
                new User(){ UserId = 1, UserName = "Asia", Password = "123456", Role = UserRole.Librarian},
                new User(){UserId = 2, UserName = "Master", Password = "136" , Role = UserRole.Administrator}
            };
        }

        public User GetById(int userId)
        {
            return _users.FirstOrDefault(user => user.UserId == userId);
        }

        public User GetByUsername(string username)
        {
            var user = _users.FirstOrDefault(user => user.UserName.Equals(username));
            return user;
        }

        public void Add(User user)
        {
            _users.Add(user);
        }

        public void Update(User user)
        {
            var existingUser = GetById(user.UserId);

            if (existingUser != null)
            {
                existingUser.UserName = user.UserName;
                existingUser.Password = user.Password;
                existingUser.Role = user.Role;
            }
        }

    }
}
