using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Middlewares
{
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
                await context.Response.WriteAsync("Icazen yoxdu e kasib");
                return;
            }
            await next(context);
        }
    }
}
