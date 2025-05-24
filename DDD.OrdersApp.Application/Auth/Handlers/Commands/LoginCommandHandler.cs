using DDD.OrdersApp.Application.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using DDD.OrdersApp.Infrastructure.Orders.Repositories;
using System.IdentityModel.Tokens.Jwt;
using DDD.OrdersApp.Application.Auth.DTOs;

namespace DDD.OrdersApp.Application.Auth.Handlers.Commands
{
    public class LoginCommandHandler : IHandler<LoginRequestDto, Result<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public LoginCommandHandler(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;          
            _configuration = configuration;
        }

        public async Task<Result<LoginResponse>> HandleAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user == null)
                return "Invalid credentials";
      
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                 return "Invalid credentials";

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            return new LoginResponse() { Token = new JwtSecurityTokenHandler().WriteToken(token) , Expires = token.ValidTo };
        }
    }


}
