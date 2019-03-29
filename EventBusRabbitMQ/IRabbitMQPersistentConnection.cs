using System;
using RabbitMQ.Client;

namespace EventBusRabbitMQProvider
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
