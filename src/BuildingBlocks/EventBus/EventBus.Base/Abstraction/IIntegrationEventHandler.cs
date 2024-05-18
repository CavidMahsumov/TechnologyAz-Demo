using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IIntegrationEventHandler<T>:IntegrationEventHandler where T:IntegrationEvent
    {
        Task Handle(T @event);
    }

    public interface IntegrationEventHandler
    {

    }
}
