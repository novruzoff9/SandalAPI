using Microsoft.AspNetCore.Http;
using Shared.ResultTypes;
using System.Text.Json;

namespace Shared.Middlewares;

public class RestrictAccessMiddleware
{
    private readonly RequestDelegate next;
    public RestrictAccessMiddleware(RequestDelegate next)
    {
        this.next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var allowedPaths = new List<string>
        {
            "/connect/token",
            "/.well-known/openid-configuration",
        };

        if (allowedPaths.Any(path => context.Request.Path.StartsWithSegments(path)))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.ContainsKey("X-Internal-Request") || context.Request.Headers["X-Internal-Request"] != "true")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            var response = Response<string>.Fail("Icazə yoxdur", StatusCodes.Status403Forbidden);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));

            return;
        }
        await next(context);
    }
}
