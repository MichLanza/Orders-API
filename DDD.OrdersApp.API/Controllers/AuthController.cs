using DDD.OrdersApp.Application.Common;
using Microsoft.AspNetCore.Mvc;
using DDD.OrdersApp.Application.Auth.DTOs;

namespace DDD.OrdersApp.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IHandler<RegisterUserDto, Result<bool>> _registerHandler;
        private readonly IHandler<LoginRequestDto, Result<LoginResponse>> _loginHandler;

        public AuthController(
            IHandler<RegisterUserDto, Result<bool>> registerHandler,
            IHandler<LoginRequestDto, Result<LoginResponse>> loginHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var result = await _registerHandler.HandleAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> GenerateToken([FromBody] LoginRequestDto dto)
        {
            var result = await _loginHandler.HandleAsync(dto);
            if (!result.IsSuccess)
                return Unauthorized(result.Error);
            return Ok(result.Value);
        }
    }
}