using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;
using OrderService.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.AggregateModels.OrderAggregate
{
    public class Order:BaseEntity,IAggregateRoot
    {
        public DateTime OrderDate { get; private set; }
        public int Quantity { get; private set; }
        public string Description { get; private set; }
        public Guid? BuyerId { get; private set; }
        public Buyer Buyer { get; private set; }
        public Address Adress { get; private set; }
        private int orderStatusId;
        public OrderStatus OrderStatus { get; private set; }
        private readonly List<OrderItem> _orderItems;
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
        public Guid? PaymentMethodId { get; private set; }
        public Order()
        {
            Id=Guid.NewGuid();
            _orderItems= new List<OrderItem>();
        }
        public Order(string username,Address adress,int cartTypeId,string cardNumber,string cardSecurityNumber,string cardHolderName,DateTime cardExpiration,Guid? paymentMethodId,Guid?buyerId=null):this()
        {
            BuyerId = buyerId;
            orderStatusId = OrderStatus.Submitted.Id;
            OrderDate = DateTime.Now;
            Adress = adress;
            PaymentMethodId = paymentMethodId;

            AddOrderStartedDomainEvent(username, cartTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);
        }
        private void AddOrderStartedDomainEvent(string username,int cardtypeId,string cardNumber,string cartdSecurityNumber,string cardHolderName,DateTime cardExpiration)
        {
            var orderStartedDomainEvent = new OrderStartedDomainEvent(username, cardtypeId, cardNumber, cartdSecurityNumber, cardHolderName, cardExpiration,this);

            this.AddDomainEvent(orderStartedDomainEvent);
        }
        public void AddOrderItem(int productId,string productName,decimal unitprice,string pictureUrl,int Units = 1)
        {
            var orderItem = new OrderItem(productId,productName,pictureUrl,unitprice,Units);
            _orderItems.Add(orderItem);
        }
        public void SetBuyerId(Guid buyerId)
        {
            BuyerId = buyerId;
        }
        public void SetPaymentMehodId(Guid paymentMethodId)
        {
            PaymentMethodId = paymentMethodId;
        }
    }
}
