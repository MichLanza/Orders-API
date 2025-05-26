using DDD.OrdersApp.Domain.Users.Entities;

namespace DDD.OrdersApp.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User> AddAsync(User user);
        Task<bool> ExistsAsync(string username);
    }
}
