using Blazored.LocalStorage;
using WebApp.Blazor.Application.Services.Dtos;
using WebApp.Blazor.Application.Services.Interfaces;
using WebApp.Blazor.Domain.Models.ViewModels;
using WebApp.Extensions;

namespace WebApp.Blazor.Application.Services
{
    public class BasketService:IBasketService
    {
        private readonly HttpClient _apiClient;
        private readonly IIdentityService _identityServices;
        private readonly ILogger<BasketService> _logger;
        private readonly ISyncLocalStorageService syncLocalStorageService;

        public BasketService(HttpClient apiClient, IIdentityService identityServices, ILogger<BasketService> logger, ISyncLocalStorageService syncLocalStorageService)
        {
            _apiClient = apiClient;
            _identityServices = identityServices;
            _logger = logger;
            this.syncLocalStorageService = syncLocalStorageService;
        }

        public async Task AddItemToBasket(int productId)
        {
            var model = new
            {
                CatalogItemId = productId,
                Quantity = 1,
                BasketId = _identityServices.GetUserName()
            };
            var token = syncLocalStorageService.GetToken();
            await _apiClient.PostAsync("basket/items", model,token);
        }

        public Task Checkout(BasketDTO basket)
        {
            var token = syncLocalStorageService.GetToken();
            return _apiClient.PostAsync("/basket/checkout", basket, token);
        }

        public async Task<Basket> GetBasket()
        {
            var res=await _apiClient.GetResponseAsync<Basket>("basket/"+_identityServices.GetUserName());
            return res ?? new Basket() { BuyerId = _identityServices.GetUserName() };

        }

        public async Task<Basket> UpdateBasket(Basket basket)
        {
            var res = await _apiClient.PostGetResponseAysnc<Basket, Basket>("basket/update", basket);
            return res;
        }
    }
}
