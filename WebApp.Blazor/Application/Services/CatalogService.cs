using WebApp.Blazor.Application.Services.Interfaces;
using WebApp.Blazor.Domain.Models;
using WebApp.Blazor.Domain.Models.CatalogModels;
using WebApp.Extensions;

namespace WebApp.Blazor.Application.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient apiClient;

        public CatalogService(HttpClient apiClient)
        {
            this.apiClient = apiClient;
        }
        public async Task<PaginatedItemViewModel<CatalogItem>> GetCatalogItem()
        {
           var res= await apiClient.GetResponseAsync<PaginatedItemViewModel<CatalogItem>>("/catalog/items");
            return res;

        }
    }
}
