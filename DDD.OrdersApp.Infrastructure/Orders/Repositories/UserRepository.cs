using DDD.OrdersApp.Domain.Interfaces.Repositories;
using DDD.OrdersApp.Domain.Users.Entities;
using DDD.OrdersApp.Infrastructure.Orders.Data;
using Microsoft.EntityFrameworkCore;

namespace DDD.OrdersApp.Infrastructure.Orders.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly OrderDbContext _context;
        public UserRepository(OrderDbContext context)
        {
            _context = context;
        }
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}
