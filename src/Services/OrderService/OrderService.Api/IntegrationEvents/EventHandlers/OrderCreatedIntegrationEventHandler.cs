using EventBus.Base.Abstraction;
using MediatR;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Application.Features.Commands.CreateOrder;

namespace OrderService.Api.IntegrationEvents.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly IMediator mediator;
        private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

        public OrderCreatedIntegrationEventHandler(IMediator mediator, ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            this.mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            _logger.LogInformation("Handling integration event : {IntegrationEventId} at {AppName} -({@IntegrationEvent})", typeof(Program).Namespace,@event);
            var createOrderCommand = new CreateOrderCommand(@event.Basket.Items, @event.UserId, @event.UserName, @event.City, @event.Street, @event.State, @event.Country, @event.ZipCode, @event.CardNumber, @event.CardHolderName, @event.CardExipiration, @event.CardSecurityNumber, @event.CardTypeId);

            await mediator.Send(createOrderCommand);
        }
    }
}
