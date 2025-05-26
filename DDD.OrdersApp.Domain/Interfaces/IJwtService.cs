using DDD.OrdersApp.Domain.Users.Entities;

namespace DDD.OrdersApp.Domain.Interfaces
{
    public interface IJwtService
    {
        TokenInfo CreateToken(User user);
    }
}
