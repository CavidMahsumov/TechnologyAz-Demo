using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistentConnection : IDisposable
    {
        private IConnection _connection;
        private readonly IConnectionFactory _connectionfactory;
        private readonly int retrycount;
        private object lock_object=new object();
        private bool _disposed;
        public RabbitMQPersistentConnection(IConnectionFactory factory,int retrycount=5)
        {
            this._connectionfactory = factory;
            this.retrycount = retrycount;
        }
        public bool isConnected => _connection != null && _connection.IsOpen;

       
        public IModel CreateModel()
        {
            return _connection.CreateModel();
        }
        public void Dispose()
        {
            _disposed=true;
            _connection.Dispose();
        }
        public bool TryConnect()
        {
            lock (lock_object)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(retrycount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {

                    });

                policy.Execute(() =>

                    _connection = _connectionfactory.CreateConnection()
                ) ;

                if (isConnected)
                {
                    _connection.ConnectionShutdown += Connection_ConnectionShutdown;
                    _connection.CallbackException += Connection_CallbackException;
                    _connection.ConnectionBlocked += Connection_ConnectionBlocked;
                    return true;
                }
                return false;
            }
        }

        private void Connection_ConnectionBlocked(object? sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            if (_disposed) return ;
            TryConnect();
        }

        private void Connection_CallbackException(object? sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (_disposed) return;
            TryConnect();
        }

        private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            if (_disposed) return;
            TryConnect();
        }
    }
}
