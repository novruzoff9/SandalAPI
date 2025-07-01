using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shared.ResultTypes;
using Shared.Services;
using System;
using System.Linq;
using System.Text;

namespace Shared.Extensions;

public static class AuthRegistration
{
    public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["AuthConfig:Secret"]));
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("roles");
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("email");
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = "roles"
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.NoResult();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        context.Response.Headers["Token-Expired"] = "true";

                        var response = Response<string>.Fail("Token expired", 401);

                        var json = System.Text.Json.JsonSerializer.Serialize(response);

                        return context.Response.WriteAsync(json);
                    }
                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var response = Response<string>.Fail(
                        "You do not have permission to access this resource.", 404
                        );

                    var json = System.Text.Json.JsonSerializer.Serialize(response);

                    return context.Response.WriteAsync(json);
                }
            };
        });
        services.AddScoped<ISharedIdentityService, SharedIdentityService>();

        return services;
    }
}
