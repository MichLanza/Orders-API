namespace DDD.OrdersApp.Infrastructure.Orders.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime OccurredOn { get; set; }
        public bool Processed { get; set; }
        public DateTime? ProcessedOn { get; set; }
    }
}
