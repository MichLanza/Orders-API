using DDD.OrdersApp.Application.Common;
using DDD.OrdersApp.Domain.Users.Entities;
using DDD.OrdersApp.Application.Auth.DTOs;
using DDD.OrdersApp.Domain.Interfaces.Repositories;
using DDD.OrdersApp.Domain.Interfaces;

namespace DDD.OrdersApp.Application.Auth.Handlers.Commands
{
    public class RegisterUserCommandHandler : IHandler<RegisterUserDto, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryption _encryption;
        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IEncryption encryption)
        {
            _userRepository = userRepository;
            _encryption = encryption;
        }
        public async Task<Result<bool>> HandleAsync(RegisterUserDto dto)
        {
            if (await _userRepository.ExistsAsync(dto.Username))
                return "Username already exists";
            var user = new User { Username = dto.Username };
            user.PasswordHash = _encryption.Encrypt(dto.Password);
            await _userRepository.AddAsync(user);
            return true;
        }
    }

}