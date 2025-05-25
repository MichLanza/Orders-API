using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DDD.OrdersApp.Infrastructure.Kafka
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly string _bootstrapServers;
        private readonly string _topic;
        private readonly string _groupId;

        public KafkaConsumerService(string bootstrapServers, string topic, string groupId = "orders-consumer-group")
        {
            _bootstrapServers = bootstrapServers;
            _topic = topic;
            _groupId = groupId;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Se utiliza un timeout para evitar llamadas bloqueantes indefinidas.
                    var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(100));
                    if (consumeResult != null && !string.IsNullOrEmpty(consumeResult.Message?.Value))
                    {
                        Log.Information("Mensaje recibido en Kafka en topic '{Topic}': {Message}", _topic, consumeResult.Message.Value);
                    }
                }
                catch (ConsumeException ex)
                {
                    Log.Error(ex, "Error al consumir mensaje de Kafka");
                }
                // Opcional: Breve retardo para evitar un ciclo muy intenso
                await Task.Delay(50, stoppingToken);
            }

            consumer.Close();
            await Task.CompletedTask;
        }
    }
}