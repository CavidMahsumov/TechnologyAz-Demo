using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ:EventBus.Base.Events.BaseEventBus
    {
        RabbitMQPersistentConnection _persistentConnection;
        private readonly IConnectionFactory _connetionFactory;
        private readonly IModel _consumerChannel;
        public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
            if (config.Connection != null)
            {
                
                var connJson = JsonConvert.SerializeObject(_config.Connection, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                });
                _connetionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);

            }
            else
            {
                _connetionFactory=new ConnectionFactory();
            }
            _persistentConnection = new RabbitMQPersistentConnection(_connetionFactory, config.ConnectionRetryCount);
            _consumerChannel = CreateConsumerChanel();
            _eventBusSubscriptionManager.OnEventRemoved += EventBusSubscriptionManager_OnEventRemoved;
        }

        private void EventBusSubscriptionManager_OnEventRemoved(object? sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if (!_persistentConnection.isConnected)
            {
                _persistentConnection.TryConnect();
            }

            //using var channel=_persistentConnection.CreateModel();
            _consumerChannel.QueueUnbind(queue: eventName,
                exchange:_config.DefaultTopicName,
                routingKey:eventName
                
                );
            if (!_eventBusSubscriptionManager.isEmpty)
            {
                _consumerChannel.Close();
            }
        }

        public override void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.isConnected)
            {
                _persistentConnection.TryConnect();
            }
            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_config.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) => { 
                        //logging
                });
            var eventName = @event.GetType().Name;
            eventName=ProcessEventName(eventName);
            _consumerChannel.ExchangeDeclare(exchange:_config.DefaultTopicName,type:"direct");


            var message = JsonConvert.SerializeObject(@event);
            var body=Encoding.UTF8.GetBytes(message);

            policy.Execute(() => {

                var properties = _consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2;

                //_consumerChannel.QueueDeclare(queue: GetSubName(eventName),//ensure queue exists while publishing
                //     durable:true,
                //     exclusive:false,
                //     autoDelete:false,
                //     arguments:null
                //    );

                //_consumerChannel.QueueBind(queue: GetSubName(eventName),
                //    exchange: _config.DefaultTopicName,
                //    routingKey: eventName
                //);


                _consumerChannel.BasicPublish(
                    exchange: _config.DefaultTopicName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                    );
            
            });

        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name; //example OrderCreatedIntegrationEvent
            eventName = ProcessEventName(eventName); //OrderCreated;

            if (!_eventBusSubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                if (!_persistentConnection.isConnected)
                {
                    _persistentConnection.TryConnect();
                }
                _consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                        durable:true,
                        exclusive:false,
                        autoDelete:false,
                        arguments:null
                    );

                _consumerChannel.QueueBind(queue: GetSubName(eventName),
                       exchange:_config.DefaultTopicName,
                       routingKey:eventName
                    );
            }
            _eventBusSubscriptionManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);

        }

        public override void UnSubscribe<T, TH>()
        {
            _eventBusSubscriptionManager.RemoveSubscription<T, TH>();   
        }
        private IModel CreateConsumerChanel()
        {
            if (!_persistentConnection.isConnected)
            {
                _persistentConnection.TryConnect();
            }
            var channel = _persistentConnection.CreateModel();
            channel.ExchangeDeclare(exchange: _config.DefaultTopicName, type: "direct");
            return channel;
        }
        private void StartBasicConsume(string eventName)
        {
            if (_consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(_consumerChannel);
                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(
                    queue:GetSubName(eventName),
                    autoAck:false,
                    consumer:consumer
                    );
            }
        }

        private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            eventName = ProcessEventName(eventName);    
            var message = Encoding.UTF8.GetString(e.Body.Span);
            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                //logging

            }
            _consumerChannel.BasicAck(e.DeliveryTag, multiple: false);
        }
    }
}
