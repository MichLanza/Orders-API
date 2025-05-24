using DDD.OrdersApp.Infrastructure.Cache;
using DDD.OrdersApp.Application.Common;
using DDD.OrdersApp.Infrastructure.Orders.Repositories;
using DDD.OrdersApp.Domain.Orders.Entities;
using DDD.OrdersApp.Application.Orders.DTOs;

namespace DDD.OrdersApp.Application.Orders.Handlers.Queries
{
    public class GetOrderQueryHandler : IHandler<GetOrderQuery, Result<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly RedisCacheService _redisCache;

        public GetOrderQueryHandler(IOrderRepository orderRepository, RedisCacheService redisCache)
        {
            _orderRepository = orderRepository;
            _redisCache = redisCache;
        }

        public async Task<Result<OrderDto>> HandleAsync(GetOrderQuery query)
        {
            var cached = _redisCache.GetString($"order:{query.Id}");
            if (!string.IsNullOrEmpty(cached))
            {
                var order = System.Text.Json.JsonSerializer.Deserialize<Order>(cached);
                if (order != null)
                    return new OrderDto { Id = order.Id, CustomerName = order.CustomerName, CreatedAt = order.CreatedAt };
            }
            var dbOrder = await _orderRepository.GetByIdAsync(query.Id);
            if (dbOrder == null) return "Order not found";
            return new OrderDto { Id = dbOrder.Id, CustomerName = dbOrder.CustomerName, CreatedAt = dbOrder.CreatedAt };
        }
    }
}