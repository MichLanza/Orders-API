using DDD.OrdersApp.Domain.Orders.Entities;

namespace DDD.OrdersApp.Domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);
    }
}
