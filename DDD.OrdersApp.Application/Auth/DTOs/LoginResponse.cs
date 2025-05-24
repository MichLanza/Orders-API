namespace DDD.OrdersApp.Application.Auth.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
