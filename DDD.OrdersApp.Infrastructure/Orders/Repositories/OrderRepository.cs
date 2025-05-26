using DDD.OrdersApp.Domain.Interfaces.Repositories;
using DDD.OrdersApp.Domain.Orders.Entities;
using DDD.OrdersApp.Infrastructure.Orders.Data;

namespace DDD.OrdersApp.Infrastructure.Orders.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;
        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }
        public async Task<Order> AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders.FindAsync(id);
        }
    }
}
