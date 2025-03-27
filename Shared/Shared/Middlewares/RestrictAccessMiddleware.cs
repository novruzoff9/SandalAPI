using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
            if (context.Request.Headers["Referrer"] != "Sandal-Gateway")
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Icazen yoxdu e kasib");
                return;
            }
            await next(context);
        }
    }
}
