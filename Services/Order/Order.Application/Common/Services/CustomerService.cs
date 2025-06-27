using Order.Application.Common.DTOs.Customer;
using Shared.Extensions.Redis;

namespace Order.Application.Common.Services;

public class CustomerService : ICustomerService
{
    private IRedisCacheService _redisCacheService;
    private ISharedIdentityService _sharedIdentityService;

    public CustomerService(IRedisCacheService redisCacheService, ISharedIdentityService sharedIdentityService)
    {
        _redisCacheService = redisCacheService;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<string> GetCustomerFullNameAsync(string customerId)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        string cacheKey = $"customer:{companyId}:{customerId}";
        var customer = await _redisCacheService.GetAsync<CustomerRedisDto>(cacheKey);
        if (customer.IsSuccess)
        {
            return $"{customer.Data.FirstName} {customer.Data.LastName}";
        }
        return string.Empty;
    }
}
