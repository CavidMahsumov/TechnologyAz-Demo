using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.IntegrationEvents
{
    public class OrderStartedIntegrationEvent:IntegrationEvent
    {
        public OrderStartedIntegrationEvent(string userName) => UserName = userName;

        public string UserName { get; set; }
    }
}
