using Web.ApiGateway.Extensions;
using Web.ApiGateway.Models.Catalog;
using Web.ApiGateway.Services.Interfaces;

namespace Web.ApiGateway.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public CatalogService(IHttpClientFactory httpClientFactory,IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }
        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            var client = httpClientFactory.CreateClient("catalog");
            var res = await client.GetResponseAsync<CatalogItem>("/items/"+id);

            return res;
        }

        public Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
        {
            //var client = httpClientFactory.CreateClient("catalog");
            //var res = await client.GetResponseAsync<CatalogItem>("/items/" + id);

            //return res;
            return null;
        }
    }
}
