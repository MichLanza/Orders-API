namespace DDD.OrdersApp.Domain.Orders.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
