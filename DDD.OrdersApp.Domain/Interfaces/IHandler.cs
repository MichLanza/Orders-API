namespace DDD.OrdersApp.Domain.Interfaces
{
    public interface IHandler<TRequest, TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request);
    }
}
