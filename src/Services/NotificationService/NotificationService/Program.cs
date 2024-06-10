// See https://aka.ms/new-console-template for more information
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.EventHandlers;
using PaymentService.Api.IntegrationEvents.Events;
using System.Runtime.CompilerServices;
using static EventBus.Base.EventBusConfig;



ServiceCollection notificationService=new ServiceCollection();
ConfigureService(services: notificationService);

var sp = notificationService.BuildServiceProvider();
IEventBus eventbus=sp.GetRequiredService<IEventBus>();

eventbus.Subscribe<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();
eventbus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();

Console.WriteLine("Application is Running . . . ");

Console.ReadLine();
void ConfigureService(ServiceCollection services)
{
    services.AddLogging(configure => configure.AddConsole());
    services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
    services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();

    services.AddSingleton<IEventBus>(sp =>
    {
        EventBusConfig config = new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            SubscriberClientAppName = "NotificationService",
            EventbusType = EventBusType.RabbitMQ
        };
        return EventBusFactory.Create(config, sp);
    });

}