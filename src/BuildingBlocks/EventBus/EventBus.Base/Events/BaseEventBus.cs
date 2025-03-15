using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base.Events;

public abstract class BaseEventBus : IEventBus
{
    public readonly IServiceProvider _serviceProvider;
    public readonly IEventBusSubscriptionManager _subsManager;

    public EventBusConfig _config { get; private set; }

    protected BaseEventBus(IServiceProvider serviceProvider, EventBusConfig config)
    {
        _serviceProvider = serviceProvider;
        _subsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        _config = config;
    }

    public async Task<bool> ProcessEvent(string eventName, string message)
    {
        eventName = ProcessEventName(eventName);

        var processed = false;
        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            var subscriptions = _subsManager.GetHandlersForEvent(eventName);

            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (var subscription in subscriptions)
                {
                    var handler = _serviceProvider.GetService(subscription.HandlerType);
                    if (handler == null) continue;

                    var eventType = _subsManager.GetEventTypeByName($"{_config.EventNamePrefix}{eventName}{_config.EventNameSuffix}");
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);


                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await Task.Yield();
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }
            processed = true;
        }
        return processed;
    }


    public virtual string ProcessEventName(string eventName)
    {
        if (_config.DeleteEventPrefix)
        {
            eventName = eventName.TrimStart(_config.EventNamePrefix.ToArray());
        }
        if (_config.DeleteEventSuffix)
        {
            eventName = eventName.TrimEnd(_config.EventNameSuffix.ToArray());
        }
        return eventName;
    }

    public virtual string GetSubName(string eventName)
    {
        return $"{_config.SubscribeClientAppName}.{ProcessEventName(eventName)}";
    }

    public virtual void Dispose()
    {
        _config = null;
    }
    public abstract void Publish(IntegrationEvent @event);

    public abstract void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    public abstract void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

}
