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

namespace EventBus.RabbitMQ;

public class EventBusRabbitMQ : BaseEventBus
{
    RabbitMQPersistentConnection _persistentConnection;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IModel _consumerChannel;

    public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
    {
        if (config.Connection != null)
        {
            var connJson = JsonConvert.SerializeObject(config.Connection, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            _connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);
        }
        else
            _connectionFactory = new ConnectionFactory();
        _persistentConnection = new RabbitMQPersistentConnection(_connectionFactory, config.ConnectionRetryCount);
        _consumerChannel = CreateConsumerChannel();
        _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
    }

    private void SubsManager_OnEventRemoved(object? sender, string eventName)
    {
        eventName = ProcessEventName(eventName);

        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }


        _consumerChannel.QueueUnbind(queue: eventName,
            exchange: _config.DefaultTopicName,
            routingKey: eventName);
        if (!_subsManager.IsEmpty)
        {
            return;
        }
        _consumerChannel.Close();
    }

    public override void Publish(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_config.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                //logging
            });

        var eventName = @event.GetType().Name;
        eventName = ProcessEventName(eventName);

        _consumerChannel.ExchangeDeclare(exchange: _config.DefaultTopicName, type: "direct");
        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        policy.Execute(() =>
        {
            var properties = _consumerChannel.CreateBasicProperties();
            properties.DeliveryMode = 2;


            _consumerChannel.BasicPublish(exchange: _config.DefaultTopicName,
                routingKey: eventName,
                mandatory: true,
                basicProperties: properties,
                body: body);
        });

    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        if (!_subsManager.HasSubscriptionsForEvent(eventName))
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _consumerChannel.QueueBind(queue: GetSubName(eventName),
                exchange: _config.DefaultTopicName,
                routingKey: eventName);
        }

        _subsManager.AddSubscription<T, TH>();
        StartBasicConsume(eventName);
    }

    public override void Unsubscribe<T, TH>()
    {
        _subsManager.RemoveSubscription<T, TH>();
    }

    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
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
            _consumerChannel.BasicConsume(queue: GetSubName(eventName),
                autoAck: false,
                consumer: consumer);
        }
    }

    private void Consumer_Received(object? sender, BasicDeliverEventArgs e)
    {
        var eventName = e.RoutingKey;
        var message = Encoding.UTF8.GetString(e.Body.Span);
        eventName = ProcessEventName(eventName);

        try
        {
            ProcessEvent(eventName, message).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            //logging
            throw;
        }

        _consumerChannel.BasicAck(e.DeliveryTag, multiple: false);
    }
}
