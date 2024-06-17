using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Core.Domain.Models;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Net;

namespace BasketService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly IIdentityService _identityService;
        private readonly IEventBus _eventBus;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IBasketRepository repository, IIdentityService identityService, IEventBus eventBus, ILogger<BasketController> logger)
        {
            this._repository = repository;
            this._identityService = identityService;
            _eventBus = eventBus;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Basket Service is Up and Running");
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket),(int)HttpStatusCode.OK)]
        public async Task<ActionResult>UpdateBasketAsync(string id)
        {
            var basket=await _repository.GetBasketAsync(id);
            return Ok(basket ?? new CustomerBasket(id));
        }
        [HttpPost]
        [Route("update")]
        [ProducesResponseType(typeof(CustomerBasket),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody]CustomerBasket value)
        {
            return Ok(await _repository.UpdateBasketAsync(value));
        }
        [Route("additem")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        [HttpPost]
        public async Task<ActionResult> AddItemToBasket([FromBody]BasketItem basketitem)
        {
            var userId = _identityService.GetUserName().ToString();
            var basket = await _repository.GetBasketAsync(userId);

            if (basket == null)
            {
                basket=new CustomerBasket(userId);
            }
            basket.Items.Add(basketitem);
            await _repository.UpdateBasketAsync(basket);
            return Ok();
        }

    }
}
