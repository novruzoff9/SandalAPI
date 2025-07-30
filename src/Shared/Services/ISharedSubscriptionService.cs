using Shared.DTOs.Subscription;
using Shared.Extensions.Redis;

namespace Shared.Services;

public interface ISharedSubscriptionService
{
    Task<string> GetSubscriptionIdAsync();
    Task<string> GetSubscriptionNameAsync();
    Task<string> GetSubscriptionCodeAsync();
    Task<DateTime> GetSubscriptionExpireAsync();
    Task<CompanySubscriptionRedisDto> GetSubscriptionAsync();
    Task<string> GetSubscriptionOfCompanyAsync(string companyId);
}

public class SharedSubscriptionService : ISharedSubscriptionService
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly ISharedIdentityService _sharedIdentityService;

    public SharedSubscriptionService(IRedisCacheService redisCacheService, ISharedIdentityService sharedIdentityService)
    {
        _redisCacheService = redisCacheService;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<CompanySubscriptionRedisDto> GetSubscriptionAsync()
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        string cacheKey = $"subscriptions:{companyId}";
        var subscription = await _redisCacheService.GetAsync<CompanySubscriptionRedisDto>(cacheKey);
        if(subscription.IsSuccess)
        {
            return subscription.Data;
        }
        return new CompanySubscriptionRedisDto(string.Empty, string.Empty, string.Empty, DateTime.MinValue);
    }

    public async Task<string> GetSubscriptionCodeAsync()
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        string cacheKey = $"subscriptions:{companyId}";
        var subscription = await _redisCacheService.GetAsync<CompanySubscriptionRedisDto>(cacheKey);
        if (subscription.IsSuccess)
        {
            return subscription.Data.PackageCode;
        }
        return string.Empty;
    }

    public async Task<DateTime> GetSubscriptionExpireAsync()
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        string cacheKey = $"subscriptions:{companyId}";
        var subscription = await _redisCacheService.GetAsync<CompanySubscriptionRedisDto>(cacheKey);
        if (subscription.IsSuccess)
        {
            return subscription.Data.ExpiredTime;
        }
        return new DateTime();
    }

    public async Task<string> GetSubscriptionIdAsync()
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        string cacheKey = $"subscriptions:{companyId}";
        var subscription = await _redisCacheService.GetAsync<CompanySubscriptionRedisDto>(cacheKey);
        if (subscription.IsSuccess)
        {
            return subscription.Data.PackageId;
        }
        return string.Empty;
    }

    public async Task<string> GetSubscriptionNameAsync()
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        string cacheKey = $"subscriptions:{companyId}";
        var subscription = await _redisCacheService.GetAsync<CompanySubscriptionRedisDto>(cacheKey);
        if (subscription.IsSuccess)
        {
            return subscription.Data.PackageName;
        }
        return string.Empty;
    }

    public async Task<string> GetSubscriptionOfCompanyAsync(string companyId)
    {
        string cacheKey = $"subscriptions:{companyId}";
        var subscription = await _redisCacheService.GetAsync<CompanySubscriptionRedisDto>(cacheKey);
        if (subscription.IsSuccess)
        {
            return subscription.Data.PackageName;
        }
        return string.Empty;
    }
}