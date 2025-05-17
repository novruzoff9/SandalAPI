using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using EventBus.Base.Abstraction;
using EventBus.Base;
using RabbitMQ.Client;
using EventBus.Factory;

namespace Subscription.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddSingleton<IEventBus>(options =>
        {
            EventBusConfig config = new()
            {
                ConnectionRetryCount = 3,
                EventNameSuffix = "IntegrationEvent",
                SubscribeClientAppName = "OrganizationService",
                EventBusType = EventBusType.RabbitMQ,
                DefaultTopicName = "SandalEventBus",
                Connection = new ConnectionFactory()
                {
                    HostName = configuration["EventBus:HostName"],
                }
            };

            return EventBusFactory.Create(config, options);
        });


        return services;
    }
}
