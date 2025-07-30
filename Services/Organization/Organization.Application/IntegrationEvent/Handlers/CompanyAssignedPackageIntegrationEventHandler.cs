using EventBus.Base.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTOs.Subscription;
using Shared.Events;
using Shared.Extensions.Redis;

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
            var companySubscriptionRedisDto = new CompanySubscriptionRedisDto(@event.PackageId, @event.PackageName, @event.PackageCode, @event.ExpiredTime);
            await redisCacheService.SetAsync(cacheKey, companySubscriptionRedisDto);
        }
    }
}
