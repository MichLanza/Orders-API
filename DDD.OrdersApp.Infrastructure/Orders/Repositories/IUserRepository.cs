using DDD.OrdersApp.Domain.Users.Entities;

namespace DDD.OrdersApp.Infrastructure.Orders.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User> AddAsync(User user);
        Task<bool> ExistsAsync(string username);
    }
}
