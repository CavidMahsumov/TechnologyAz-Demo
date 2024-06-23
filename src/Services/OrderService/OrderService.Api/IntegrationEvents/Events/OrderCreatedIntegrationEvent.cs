using EventBus.Base.Events;
using OrderService.Domain.Models;

namespace OrderService.Api.IntegrationEvents.Events
{
    public class OrderCreatedIntegrationEvent:IntegrationEvent
    {
        public OrderCreatedIntegrationEvent(string userId, string userName, int orderNumber, string city, string street, string state, string country, string zipCode, string cardNumber, string cardHolderName, DateTime cardExipiration, string cardSecurityNumber, int cardTypeId, string buyer, Guid requestId, CustomerBasket basket)
        {
            UserId = userId;
            UserName = userName;
            OrderNumber = orderNumber;
            City = city;
            Street = street;
            State = state;
            Country = country;
            ZipCode = zipCode;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardExipiration = cardExipiration;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeId = cardTypeId;
            Buyer = buyer;
            RequestId = requestId;
            Basket = basket;
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public int OrderNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string  State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime CardExipiration { get; set; }
        public string CardSecurityNumber { get; set; }
        public int CardTypeId { get; set; }
        public string Buyer { get; set; }
        public Guid RequestId { get; set; }
        public CustomerBasket Basket { get; set; }
    }   
}
