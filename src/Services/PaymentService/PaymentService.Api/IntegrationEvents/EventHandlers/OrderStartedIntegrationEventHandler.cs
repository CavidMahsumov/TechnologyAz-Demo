using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using PaymentService.Api.IntegrationEvents.Events;

namespace PaymentService.Api.IntegrationEvents.EventHandlers
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly IConfiguration _configuration;
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderStartedIntegrationEventHandler> _logger;

        public OrderStartedIntegrationEventHandler(IConfiguration configuration, IEventBus eventBus, ILogger<OrderStartedIntegrationEventHandler> logger)
        {
            _configuration = configuration;
            _eventBus = eventBus;
            _logger = logger;
        }

        public Task Handle(OrderStartedIntegrationEvent @event)
        {

            //Fake payment process
            string keyword = "PaymentSuccess";
            bool paymentSuccessFlag = _configuration.GetValue<bool>(keyword);
            IntegrationEvent paymentevent = paymentSuccessFlag
                ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId)
                : new OrderPaymentFailedIntegrationEvent(@event.OrderId,"This is a Fake Error Message");
            _logger.LogInformation($"OrderCreatedIntegrationEventHandler is PaymentService is fired with PaymentSuccess : {paymentSuccessFlag},orderId : {@event.OrderId}");
            _eventBus.Publish(paymentevent);
            return Task.CompletedTask;
        }
    }
}
