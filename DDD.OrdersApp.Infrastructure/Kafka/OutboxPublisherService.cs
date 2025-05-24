using Confluent.Kafka;
using DDD.OrdersApp.Infrastructure.Orders.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DDD.OrdersApp.Infrastructure.Kafka
{
    public class OutboxPublisherService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _bootstrapServers;
        private readonly string _topic;
        private readonly ILogger<OutboxPublisherService> _logger;

        public OutboxPublisherService(IServiceProvider serviceProvider, string bootstrapServers, 
            string topic, ILogger<OutboxPublisherService> logger)
        {
            _serviceProvider = serviceProvider;
            _bootstrapServers = bootstrapServers;
            _topic = topic;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                var messages = await db.OutboxMessages
                    .Where(x => !x.Processed)
                    .OrderBy(x => x.OccurredOn)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                if (messages.Count == 0)
                {
                    await Task.Delay(2000, stoppingToken);
                    continue;
                }

                var config = new ProducerConfig { BootstrapServers = _bootstrapServers };
                using var producer = new ProducerBuilder<Null, string>(config).Build();

                foreach (var msg in messages)
                {
                    try
                    {
                        await producer.ProduceAsync(_topic, new Message<Null, string> { Value = msg.Payload }, stoppingToken);
                        msg.Processed = true;
                        msg.ProcessedOn = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error publishing outbox message {msg.Id}");
                    }
                }
                await db.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
