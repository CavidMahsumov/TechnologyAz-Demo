using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Events
{
    public class IntegrationEvent
    {
        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createdDate)
        {
            Id = id;
            CreatedDate = createdDate;
        }
        public IntegrationEvent()
        {
            Id= Guid.NewGuid();
            CreatedDate= DateTime.Now;
        }
        [JsonProperty]
        public Guid  Id { get; private set; }
        [JsonProperty]
        public DateTime CreatedDate { get; private set; }

     
    }
}
