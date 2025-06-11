using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Shared.Services;

public interface ISharedIdentityService
{
    public string GetUserId { get; }
    public string GetCompanyId { get; }
    public string GetWarehouseId { get; }
    public ClaimsPrincipal GetUser { get; }
}

public class SharedIdentityService : ISharedIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SharedIdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal GetUser => _httpContextAccessor.HttpContext.User;

    public string GetUserId => GetClaimValue("sub");
    public string GetCompanyId => GetClaimValue("company");
    public string GetWarehouseId => GetClaimValue("warehouse");
    public string GetSubscription => GetClaimValue("subscription");

    private string GetClaimValue(string claimType)
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(claimType);
        return claim?.Value;
    }
}
