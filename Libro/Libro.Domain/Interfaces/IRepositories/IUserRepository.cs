﻿using Libro.Domain.Dtos;
namespace Libro.Domain.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        //Task<User> GetUserByIdAsync(string userId);
        //Task<User> GetUserByNameAsync(string username);
        Task UpdateUserAsync(string userId, string name, string email);
        Task DeleteUserAsync(string userId);
        Task<string> GetCurrentUserIdAsync();
        Task<List<UserDto>> GetAllUsersAsync();
    }
}
