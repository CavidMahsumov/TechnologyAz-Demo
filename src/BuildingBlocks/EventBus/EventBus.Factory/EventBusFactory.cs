using EventBus.AzureServiceBus;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using EventBus.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Factory
{
    public static class EventBusFactory
    {
        public static IEventBus Create(EventBusConfig config,IServiceProvider serviceProvider)
        {
            //var conn= new DefaultServiceBusPersisterConnection(config);

            return config.EventbusType switch
            {
                EventBusConfig.EventBusType.AzureServiceBus => new EventBusServiceBus(config: config, serviceProvider: serviceProvider),
                _ => new EventBusRabbitMQ(config: config, serviceProvider: serviceProvider)
            }; ; ;
        }

    }
}
