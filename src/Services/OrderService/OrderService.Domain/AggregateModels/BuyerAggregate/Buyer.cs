using OrderService.Domain.Events;
using OrderService.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.AggregateModels.BuyerAggregate
{
    public class Buyer:BaseEntity
    {
        public string Name { get; set; }
        private List<PaymentMethod> _paymentMethod;
        public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethod.AsReadOnly();

        protected Buyer()
        {
            _paymentMethod = new List<PaymentMethod>();
        }
        public Buyer(string name):this()
        {
            Name = name ?? throw new  ArgumentNullException(nameof(name));
        }

        public PaymentMethod VerifyOrAddPaymentMethod(
            int cardTypeId,string alias,string cardNumber,
            string securityNumber,string cardHolderName,DateTime expiration,Guid orderId)
        {
            var existingPayment = _paymentMethod.SingleOrDefault(p => p.IsEqualTo(cardTypeId, cardNumber, expiration));
            if (existingPayment != null)
            {
                AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));
                return existingPayment;
            }

            var payment = new PaymentMethod(alias, cardNumber, securityNumber, cardHolderName, expiration, cardTypeId);
            _paymentMethod.Add(payment);
            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));
            return payment;

        }
    }
}
