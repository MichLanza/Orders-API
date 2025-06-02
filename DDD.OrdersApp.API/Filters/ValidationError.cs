namespace DDD.OrdersApp.API.Filters
{
    internal class ValidationError
    {
        public string Attribute { get; set; }
        public string[] Messages { get; set; }
    }
}