using DDD.OrdersApp.Domain.Orders.Entities;
using DDD.OrdersApp.Domain.Users.Entities;
using DDD.OrdersApp.Infrastructure.Orders.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDD.OrdersApp.Infrastructure.Orders.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
    }
}
