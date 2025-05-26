using DDD.OrdersApp.Domain.Interfaces;
using DDD.OrdersApp.Domain.Users.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DDD.OrdersApp.Infrastructure.Jwt;

public class JwtGeneration : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtGeneration(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public TokenInfo CreateToken(User user)
    {

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
            expires: DateTime.Now.AddHours(int.Parse(_configuration["Jwt:Duration"])),
            signingCredentials: creds
        );
        return new TokenInfo() { Token = new JwtSecurityTokenHandler().WriteToken(token), Expires = token.ValidTo };
    }
}
