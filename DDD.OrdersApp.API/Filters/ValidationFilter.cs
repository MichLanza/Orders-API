using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DDD.OrdersApp.API.Filters
{
    public class ValidationFilter<T> : IAsyncActionFilter
    {
        private readonly IValidator<T> _validator;

        public ValidationFilter(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var args = context.ActionArguments.Values.OfType<T>().FirstOrDefault();

            if (args is null)
            {
                context.Result = new BadRequestObjectResult(new { errors = new { message = "Error en la petición" } });
                return;
            }

            var validationsResults = await _validator.ValidateAsync(args);

            if (!validationsResults.IsValid)
            {

                context.Result = new BadRequestObjectResult(
                    new
                    {
                        errors = validationsResults.ToDictionary().Select(validation => new
                        {
                            attribute = validation.Key,
                            messages = validation.Value
                        })
                    });
                return;
            }

            await next();
        }
    }
}