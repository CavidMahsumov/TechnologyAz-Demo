using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Web.ApiGateway.Models.Basket;
using Web.ApiGateway.Services.Interfaces;

namespace Web.ApiGateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;

        public BasketController(ICatalogService catalogService, IBasketService basketService)
        {
            _catalogService = catalogService;
            _basketService = basketService;
        }

        [HttpPost]
        [Route("itemsAdd")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddBasketItemAsync([FromBody] AddBasketItemRequest request)
        {
            if (request is null || request.Quantity == 0)
                return BadRequest("Invalid Payload");

            var item = await _catalogService.GetCatalogItemAsync(request.CatalogItemId);

            var curretbasket = await _basketService.GetById(request.BasketId);

            var product=curretbasket.Items.SingleOrDefault(i=>i.ProductId==item.Id);

            if (product != null)
                product.Quantity += request.Quantity;
            else
                curretbasket.Items.Add(new BasketDataItem()
                {
                     UnitPrice=item.Price,
                     PictureUrl=item.PictureUrl,
                     ProductId=item.Id,
                     Quantity=request.Quantity,
                     Id=Guid.NewGuid().ToString(),
                     ProductName=item.Name
                });
            await _basketService.UpadateAsync(curretbasket);
            return Ok();

        }
    }
}
