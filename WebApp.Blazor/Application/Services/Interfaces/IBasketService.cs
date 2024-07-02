using WebApp.Blazor.Application.Services.Dtos;
using WebApp.Blazor.Domain.Models.ViewModels;

namespace WebApp.Blazor.Application.Services.Interfaces
{
    public interface IBasketService
    {
        Task<Basket> GetBasket();
        Task<Basket> UpdateBasket(Basket basket);
        Task AddItemToBasket(int productId);
        Task Checkout(BasketDTO basket);
    }
}
