using DDD.OrdersApp.Application.Orders.Handlers.Commands;
using FluentValidation;

namespace DDD.OrdersApp.API.Validators.Orders
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required")
                .MaximumLength(100).WithMessage("Customer name must be at most 100 characters");
        }
    }
}
