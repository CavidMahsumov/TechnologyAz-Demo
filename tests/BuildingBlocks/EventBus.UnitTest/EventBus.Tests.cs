using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using EventBus.UnitTest.Events.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBus.UnitTest
{
    [TestClass]
    public class EventBusTests
    {
        private ServiceCollection services;

        public EventBusTests()
        {
            this.services = new ServiceCollection();
            services.AddLogging(configure=>configure.AddConsole());
        }

        [TestMethod]
        public void subscribe_event_on_rabbitmq_test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                
                return EventBusFactory.Create(GetRabbitMqConfig(), sp);
            });

            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

           // eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        }
        [TestMethod]
        public void subscribe_event_on_azure_test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                
                return EventBusFactory.Create(GetAzureConfig(), sp);
            });

            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            //eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            Task.Delay(2000).Wait();

        }

        [TestMethod]
        public void send_message_to_rabbitmq_test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {

                return EventBusFactory.Create(GetRabbitMqConfig(), sp);
            });

            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Publish(new OrderCreatedIntegrationEvent(1));

        }
        [TestMethod]
        public void send_message_to_azure_test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {

                return EventBusFactory.Create(GetAzureConfig(), sp);
            });

            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Publish(new OrderCreatedIntegrationEvent(1));

        }


        private EventBusConfig GetAzureConfig()
        {
            EventBusConfig config = new EventBusConfig()
            {

                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "TechologyazEventBus",
                EventbusType = EventBusConfig.EventBusType.AzureServiceBus,
                EventNameSuffix = "IntegrationEvent",
                EventBusConnectionString = "{past here connection string}"

            };
            return config;
        }

        private EventBusConfig GetRabbitMqConfig()
        {
            EventBusConfig config = new EventBusConfig()
            {

                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "TechologyazEventBus",
                EventbusType = EventBusConfig.EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent",

            };
            return config;
        }

    }
}