using DDD.OrdersApp.Application.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using DDD.OrdersApp.Application.Auth.DTOs;
using DDD.OrdersApp.Domain.Interfaces.Repositories;
using DDD.OrdersApp.Domain.Interfaces;

namespace DDD.OrdersApp.Application.Auth.Handlers.Commands
{
    public class LoginCommandHandler : IHandler<LoginRequestDto, Result<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryption _encryption;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(IUserRepository userRepository,
            IEncryption encryption,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _encryption = encryption;
            _jwtService = jwtService;
        }

        public async Task<Result<LoginResponse>> HandleAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user == null)
                return "Invalid credentials";

            if (!_encryption.VerifyEncryption(dto.Password, user.PasswordHash))
                return "Invalid credentials";

            var token = _jwtService.CreateToken(user);

            return new LoginResponse() { Token = token.Token, Expires = token.Expires };
        }
    }


}
