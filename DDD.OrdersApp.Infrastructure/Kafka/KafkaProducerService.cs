using Confluent.Kafka;

namespace DDD.OrdersApp.Infrastructure.Kafka
{
    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        public KafkaProducerService(string bootstrapServers)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }
        public async Task ProduceAsync(string topic, string message)
        {
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }
    }
}
