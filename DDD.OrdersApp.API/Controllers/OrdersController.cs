using Microsoft.AspNetCore.Mvc;
using DDD.OrdersApp.Application.Common;
using DDD.OrdersApp.API.Filters;
using Microsoft.AspNetCore.Authorization;
using DDD.OrdersApp.Application.Orders.Handlers.Queries;
using DDD.OrdersApp.Application.Orders.Handlers.Commands;
using DDD.OrdersApp.Application.Orders.DTOs;

namespace DDD.OrdersApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IHandler<CreateOrderCommand, Result<OrderDto>> _createOrderHandler;
        private readonly IHandler<GetOrderQuery, Result<OrderDto>> _getOrderHandler;

        public OrdersController(
            IHandler<CreateOrderCommand, Result<OrderDto>> createOrderHandler,
            IHandler<GetOrderQuery, Result<OrderDto>> getOrderHandler)
        {
            _createOrderHandler = createOrderHandler;
            _getOrderHandler = getOrderHandler;
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilter<CreateOrderCommand>))]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var result = await _createOrderHandler.HandleAsync(command);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetOrder), new { id = result.Value!.Id }, result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var result = await _getOrderHandler.HandleAsync(new GetOrderQuery { Id = id });
            if (!result.IsSuccess)
                return NotFound(result.Error);
            return Ok(result.Value);
        }
    }
}