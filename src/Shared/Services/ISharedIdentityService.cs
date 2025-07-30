using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Shared.Services;

public interface ISharedIdentityService
{
    public string GetRole { get; }
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
    public string GetRole => GetClaimValue("roles");
    public string GetWarehouseId => GetClaimValue("warehouse");

    private string GetClaimValue(string claimType)
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(claimType);
        return claim?.Value;
    }
}
