using EventBus.Base.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Order.Application.Common.DTOs.Customer;
using Shared.Events;
using Shared.Extensions.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.IntegratonEvents.Handlers;

public class CustomerCreatedIntegrationEventHandler : IIntegrationEventHandler<CustomerCreatedIntegrationEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CustomerCreatedIntegrationEventHandler> _logger;

    public CustomerCreatedIntegrationEventHandler(IServiceScopeFactory serviceScopeFactory, ILogger<CustomerCreatedIntegrationEventHandler> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task Handle(CustomerCreatedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var redisCacheService = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();

            string companyId = @event.CompanyId;
            string cacheKey = $"customer:{companyId}:{@event.CustomerId}";
            var customerRedisDto = new CustomerRedisDto(@event.FirstName, @event.LastName);
            await redisCacheService.SetAsync<CustomerRedisDto>(cacheKey, customerRedisDto);
        }
    }
}