using Shared.Extensions.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;

public interface ISharedSubscriptionService
{
    Task<string> GetSubscriptionName();
    Task<string> GetSubscriptionId();
    Task<string> GetSubscriptionOfCompany(string companyId);
    Task<DateTime> GetSubscriptionExpire();
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

    public async Task<DateTime> GetSubscriptionExpire()
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        string cacheKey = $"subscription:{companyId}";
        var subscription = await _redisCacheService.GetAsync<CompanySubscriptionRedisDto>(cacheKey);
        if (subscription.IsSuccess)
        {
            return subscription.Data.ExpiredTime;
        }
        return new DateTime();
    }

    public async Task<string> GetSubscriptionId()
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        string cacheKey = $"subscription:{companyId}";
        var subscription = await _redisCacheService.GetAsync<CompanySubscriptionRedisDto>(cacheKey);
        if (subscription.IsSuccess)
        {
            return subscription.Data.PackageId;
        }
        return string.Empty;
    }

    public async Task<string> GetSubscriptionName()
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

    public async Task<string> GetSubscriptionOfCompany(string companyId)
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

internal class CompanySubscriptionRedisDto
{
    public string PackageId { get; set; }
    public string PackageName { get; set; }
    public DateTime ExpiredTime { get; set; }
    public CompanySubscriptionRedisDto(string packageId, string packageName, DateTime expiredTime)
    {
        PackageId = packageId;
        PackageName = packageName;
        ExpiredTime = expiredTime;
    }
}