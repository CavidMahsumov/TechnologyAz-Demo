using WebApp.Blazor.Domain.Models;
using WebApp.Blazor.Domain.Models.CatalogModels;

namespace WebApp.Blazor.Application.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<PaginatedItemViewModel<CatalogItem>> GetCatalogItem();
    }
}
