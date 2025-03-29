using EventBus.Base.Abstraction;
using EventBus.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organization.Application.Common.Services;
using Organization.Application.IntegrationEvent.Handlers;
using Shared.Services;
using System.Reflection;
using EventBus.Factory;
using FluentValidation;
using Organization.Application.Common.Behaviors;

namespace Organization.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        //Automapper configuration
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        //FluentValidation configuration
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        //MediatR configuration
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddScoped<ISharedIdentityService, SharedIdentityService>();
        services.AddScoped<IExcelService, ExcelService>();

        services.AddSingleton<IEventBus>(options =>
        {
            EventBusConfig config = new()
            {
                ConnectionRetryCount = 3,
                EventNameSuffix = "IntegrationEvent",
                SubscribeClientAppName = "OrganizationService",
                EventBusType = EventBusType.RabbitMQ,
                DefaultTopicName = "SandalEventBus"
            };

            return EventBusFactory.Create(config, options);
        });


        services.AddTransient<OrderCreatedIntegrationEventHandler>();
        services.AddScoped<ShelfProductService>();

        return services;
    }
}