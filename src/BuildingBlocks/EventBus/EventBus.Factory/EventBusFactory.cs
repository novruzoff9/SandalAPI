﻿using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Factory;

public class EventBusFactory
{
    public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
    {
        return config.EventBusType switch
        {
            EventBusType.RabbitMQ => new EventBusRabbitMQ(serviceProvider, config),
            //EventBusType.AzureServiceBus => new InMemoryEventBus(serviceProvider),
            _ => throw new NotImplementedException()
        };
    }
}
