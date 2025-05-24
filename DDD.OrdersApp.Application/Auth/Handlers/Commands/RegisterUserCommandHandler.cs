using DDD.OrdersApp.Application.Common;
using DDD.OrdersApp.Infrastructure.Orders.Repositories;
using DDD.OrdersApp.Domain.Users.Entities;
using DDD.OrdersApp.Application.Auth.DTOs;

namespace DDD.OrdersApp.Application.Auth.Handlers.Commands
{
    public class RegisterUserCommandHandler : IHandler<RegisterUserDto, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<bool>> HandleAsync(RegisterUserDto dto)
        {
            if (await _userRepository.ExistsAsync(dto.Username))
                return "Username already exists";
            var user = new User { Username = dto.Username };
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            await _userRepository.AddAsync(user);
            return true;
        }
    }

}