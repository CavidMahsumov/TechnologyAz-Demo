using OrderService.Domain.Exceptions;
using OrderService.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.AggregateModels.BuyerAggregate
{
    public class PaymentMethod:BaseEntity
    {
        public string Alias { get; set; }
        public string CardNumber { get; set; }
        public string SecurityNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime Expiration { get; set; }
        public int CardTypeId { get; set; }
        public CardType CardType { get;private  set; }

        public PaymentMethod()
        {

        }

        public PaymentMethod(string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration, int cardTypeId)
        {
            CardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new OrderingDomainException(nameof(cardNumber));
            SecurityNumber = !string.IsNullOrWhiteSpace(SecurityNumber) ? SecurityNumber : throw new OrderingDomainException(nameof(SecurityNumber));
            CardHolderName = !string.IsNullOrWhiteSpace(CardHolderName) ? CardHolderName : throw new OrderingDomainException(nameof(CardHolderName));

            if (expiration < DateTime.UtcNow)
            {
                throw new OrderingDomainException(nameof(expiration));
            }
            Alias = alias;
            Expiration= expiration;
            CardTypeId = cardTypeId;
                
        }
        public bool IsEqualTo(int cartTypeId,string cardNumber,DateTime expiration)
        {
            return CardTypeId==cartTypeId
                && CardNumber==cardNumber
                && Expiration==expiration;
        }
    }
}
