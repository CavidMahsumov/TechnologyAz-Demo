using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Events
{
    public abstract class BaseEventBus : IEventBus
    {
        public readonly IServiceProvider _serviceProvider;
        public readonly IEventBusSubscriptionManager _eventBusSubscriptionManager;

        private EventBusConfig _config;


        protected BaseEventBus(IServiceProvider serviceProvider, EventBusConfig config)
        {
            _serviceProvider = serviceProvider;
            _config = config;
            _eventBusSubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        }

        public virtual string ProcessEventName(string eventName)
        {
            if (_config.DeleteEventPrefix)
            {
                eventName = eventName.TrimStart(_config.EventNamePrefix.ToArray());
            }
            if (_config.DeleteEventSuffix)
            {
                eventName = eventName.TrimEnd(_config.EventNameSuffix.ToArray());
            }
            return eventName;
        }
        public virtual string GetSubName(string eventName)
        {
            return $"{_config.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }

        public virtual void Dispose()
        {
            _config = null;
        }
        public async Task<bool>ProcessEvent(string eventName,string message)
        {
            eventName = ProcessEventName(eventName);

            var processed = false;
            if (_eventBusSubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                var subsriptions=_eventBusSubscriptionManager.GetHandlersForEvent(eventName);
                using (var scope = _serviceProvider.CreateScope())
                {
                    foreach (var subs in subsriptions)
                    {
                        var handler = _serviceProvider.GetService(subs.HandlerType);
                        if (handler==null)
                        {
                            continue;
                        }
                        var eventType = _eventBusSubscriptionManager.GetEventTypeByName($"{_config.EventNamePrefix}{eventName}{_config.EventNameSuffix}");
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        
                  
                        var concreteType=typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });

                    }
                }
                processed = true;
            }
            return processed;
        }

        public abstract void Publish(IntegrationEvent @event);

        public abstract void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        public abstract void UnSubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}
