using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus : BaseEventBus
    {
        ITopicClient _topicClient;
        private ManagementClient _managementClient;
        private ILogger _logger;
        public EventBusServiceBus(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
            _logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;
            _managementClient = new ManagementClient(_config.EventBusConnectionString);
            _topicClient = createTopicClient();
        }

        private ITopicClient createTopicClient()
        {
            if (_topicClient == null || _topicClient.IsClosedOrClosing)
            {
                _topicClient = new TopicClient(_config.EventBusConnectionString,_config.DefaultTopicName,RetryPolicy.Default);
            }

            //ensure that topic already exists
            if (!_managementClient.TopicExistsAsync(_config.DefaultTopicName).GetAwaiter().GetResult())
            {
              _managementClient.CreateTopicAsync(_config.DefaultTopicName).GetAwaiter().GetResult();
            }
            return _topicClient;
        }

        public override void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name; //example OrderCreatedIntegrationEvent
            eventName = ProcessEventName(eventName); //OrderCreated;

            var eventstr=JsonConvert.SerializeObject(@event);
            
            var bodyArr=Encoding.UTF8.GetBytes(eventstr);

            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = bodyArr,
                Label = eventName

            };
            _topicClient.SendAsync(message).GetAwaiter().GetResult();   
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);
            if (!_eventBusSubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                var subscriptionclient = CreateSubscriptionClient(eventName);
                RegisterSubscriptionClientMessageHandler(subscriptionclient);
            }
            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);
            _eventBusSubscriptionManager.AddSubscription<T, TH>();
        }

        public override void UnSubscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            try
            {
                var subscriptionClient = CreateSubscriptionClient(eventName);
                subscriptionClient
                    .RemoveRuleAsync(eventName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogWarning("The messaging entity {EventName} Could be not found", eventName);
            }

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _eventBusSubscriptionManager.RemoveSubscription<T, TH>();
        }

        private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
        {
            subscriptionClient.RegisterMessageHandler(
                 async (message, token) =>
                 {
                     var eventName = $"{message.Label}";
                     var messageData = Encoding.UTF8.GetString(message.Body);

                     if (await ProcessEvent(ProcessEventName(eventName), messageData))
                     {
                         await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                     }
                 },
                 new MessageHandlerOptions(ExceptionReceiveHandler) { MaxConcurrentCalls = 10, AutoComplete = false }

                ); 
        }

        private Task ExceptionReceiveHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex= exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            _logger.LogError(ex, "ERROR handling message : {ExceptionMessage} - Context : {@ExceptionContext}", ex.Message, context);

            return Task.CompletedTask;
        }

        private SubscriptionClient CreateSubscriptionClient(string eventName)
        {
            return new SubscriptionClient(_config.EventBusConnectionString, _config.DefaultTopicName, GetSubName(eventName));
        }

        private ISubscriptionClient CreateSubscriptionClientIfNotExist(string eventName) {

            var subClient = CreateSubscriptionClient(eventName);
            var exists=_managementClient.SubscriptionExistsAsync(_config.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();

            if (!exists)
            {
                _managementClient.CreateSubscriptionAsync(_config.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();

                RemoveDefaultRule(subClient);

            }

            CreateRuleIfNotExists(ProcessEventName(eventName), subClient);
            return subClient;
                
        }

        private void CreateRuleIfNotExists(string eventName,ISubscriptionClient subscriptionclient)
        {
            bool ruleExists; 
            try
            {
                var rule = _managementClient.GetRuleAsync(_config.DefaultTopicName, GetSubName(eventName),eventName).GetAwaiter().GetResult();
                ruleExists = rule != null;
            }
            catch (MessagingEntityNotFoundException)
            {
                //Azure Management Client doesn't have RuleExists mehtod
                ruleExists = false;
            }
            if (!ruleExists)
            {
                subscriptionclient.AddRuleAsync(new RuleDescription
                {
                    Filter=new CorrelationFilter { Label=eventName},
                    Name=eventName
                }).GetAwaiter().GetResult();
            }
        }

        private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
        {
            try
            {
                subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogWarning("The messaging entity {DefaultRuleName} Could be not found.",RuleDescription.DefaultRuleName);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _topicClient.CloseAsync().GetAwaiter().GetResult();
            _managementClient.CloseAsync().GetAwaiter().GetResult();
            _topicClient = null;
            _managementClient = null;
           
        }
    }
}
