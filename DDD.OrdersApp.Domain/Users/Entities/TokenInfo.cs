
namespace DDD.OrdersApp.Domain.Users.Entities
{
    public class TokenInfo
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}