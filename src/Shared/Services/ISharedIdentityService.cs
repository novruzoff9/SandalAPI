﻿using Microsoft.AspNetCore.Http;
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

    public string GetUserId => _httpContextAccessor.HttpContext.User.FindFirst("sub").Value;
    public string GetCompanyId => _httpContextAccessor.HttpContext.User.FindFirst("company").Value;
    public string GetWarehouseId => _httpContextAccessor.HttpContext.User.FindFirst("warehouse").Value;
    public string GetSubscription => _httpContextAccessor.HttpContext.User.FindFirst("subscription").Value;
}
