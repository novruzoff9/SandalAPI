using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Shared.Middlewares
{
    public class TokenCheckerMiddleware
    {
        private readonly RequestDelegate next;
        public TokenCheckerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            string requestPath = context.Request.Path.Value!;
            if (context.Request.Headers["Authorization"].Count == 0)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token yoxdu, get ozuve token al!");
                return;
            }
            await next(context);
        }
    }
}
