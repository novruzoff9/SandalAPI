using EventBus.Base.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Organization.Application.DTOs.Company;
using Shared.Events;
using Shared.Extensions.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.IntegrationEvent.Handlers;

public class CompanyAssignedPackIntegrationEventHandler : IIntegrationEventHandler<CompanyAssignedPackIntegrationEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CompanyAssignedPackIntegrationEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(CompanyAssignedPackIntegrationEvent @event, CancellationToken cancellationToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var redisCacheService = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();
            string companyId = @event.CompanyId;
            string cacheKey = $"subscriptions:{companyId}";
            var companySubscriptionRedisDto = new CompanySubscriptionRedisDto(@event.PackageId, @event.PackageName, @event.ExpiredTime);
            await redisCacheService.SetAsync<CompanySubscriptionRedisDto>(cacheKey, companySubscriptionRedisDto);
        }
    }
}
