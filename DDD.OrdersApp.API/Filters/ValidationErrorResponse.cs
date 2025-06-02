using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DDD.OrdersApp.API.Filters
{
    internal class ValidationErrorResponse : ModelStateDictionary
    {
        public object Errors { get; set; }
    }
}