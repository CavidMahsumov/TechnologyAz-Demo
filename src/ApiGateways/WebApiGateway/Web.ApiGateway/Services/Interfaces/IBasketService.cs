using Web.ApiGateway.Models.Basket;

namespace Web.ApiGateway.Services.Interfaces
{
    public interface IBasketService
    {
        Task<BasketData> GetById(string id);
        Task<BasketData> UpadateAsync(BasketData currentBasket);
    }
}
