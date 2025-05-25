using DDD.OrdersApp.Infrastructure.Cache;
using DDD.OrdersApp.Infrastructure.Kafka;
using DDD.OrdersApp.Application.Common;
using DDD.OrdersApp.Infrastructure.Orders.Repositories;
using DDD.OrdersApp.Application.Orders.DTOs;
using DDD.OrdersApp.Domain.Orders.Entities;

namespace DDD.OrdersApp.Application.Orders.Handlers.Commands
{
    public class CreateOrderCommandHandler : IHandler<CreateOrderCommand, Result<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly RedisCacheService _redisCache;
        private readonly KafkaProducerService _kafkaProducer;
        private readonly string _kafkaTopic;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, RedisCacheService redisCache, KafkaProducerService kafkaProducer, string kafkaTopic)
        {
            _orderRepository = orderRepository;
            _redisCache = redisCache;
            _kafkaProducer = kafkaProducer;
            _kafkaTopic = kafkaTopic;
        }

        public async Task<Result<OrderDto>> HandleAsync(CreateOrderCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.CustomerName))
                return "Customer name is required";
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = command.CustomerName,
                CreatedAt = DateTime.UtcNow
            };
            await _orderRepository.AddAsync(order);
            var serialized = System.Text.Json.JsonSerializer.Serialize(order);
            _redisCache.SetString($"order:{order.Id}", serialized, TimeSpan.FromMinutes(30));
            await _kafkaProducer.ProduceAsync(_kafkaTopic, serialized);
            return new OrderDto { Id = order.Id, CustomerName = order.CustomerName, CreatedAt = order.CreatedAt };
        }
    }
}