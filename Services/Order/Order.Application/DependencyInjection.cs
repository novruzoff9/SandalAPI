using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Order.Application.Common.Services;
using EventBus.Base.Abstraction;
using EventBus.Base;
using Order.Application.IntegratonEvents.Handlers;
using EventBus.Factory;
using RabbitMQ.Client;

namespace Order.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddScoped<ISharedIdentityService, SharedIdentityService>();
        services.AddScoped<IExcelService, ExcelService>();
        services.AddScoped<CustomerService>();

        services.AddSingleton<IEventBus>(options =>
        {
            EventBusConfig config = new()
            {
                ConnectionRetryCount = 3,
                EventNameSuffix = "IntegrationEvent",
                SubscribeClientAppName = "OrderService",
                EventBusType = EventBusType.RabbitMQ,
                DefaultTopicName = "SandalEventBus",
                Connection = new ConnectionFactory()
                {
                    HostName = configuration["EventBus:HostName"],
                }
            };

            return EventBusFactory.Create(config, options);
        });

        services.AddTransient<OrderStockNotEnoughIntegrationEventHandler>();
        services.AddTransient<CustomerCreatedIntegrationEventHandler>();
        services.AddTransient<OrderStockConfirmedIntegrationEventHandler>();

        return services;
    }
}
