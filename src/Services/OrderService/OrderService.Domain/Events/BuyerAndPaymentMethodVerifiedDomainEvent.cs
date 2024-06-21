using MediatR;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Events
{
    public class BuyerAndPaymentMethodVerifiedDomainEvent:INotification
    {
        public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod paymentMethod, Guid orderId)
        {
            Buyer = buyer;
            PaymentMethod = paymentMethod;
            OrderId = orderId;
        }

        public Buyer Buyer { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public Guid OrderId { get; private set; }

    }
}
