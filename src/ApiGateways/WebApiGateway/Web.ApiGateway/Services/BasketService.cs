using Web.ApiGateway.Extensions;
using Web.ApiGateway.Models.Basket;
using Web.ApiGateway.Services.Interfaces;

namespace Web.ApiGateway.Services
{
    public class BasketService : IBasketService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BasketService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<BasketData> GetById(string id)
        {
            var client = _httpClientFactory.CreateClient("basket");
            var res = await client.GetResponseAsync<BasketData>(id);

            
            return res ??new BasketData(id);
        }

        public async Task<BasketData> UpadateAsync(BasketData currentBasket)
        {
            var client = _httpClientFactory.CreateClient("basket");
            return await client.PostGetResponseAysnc<BasketData, BasketData>("update", currentBasket);
        }
    }
}
