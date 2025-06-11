using Microsoft.AspNetCore.Http;
using Shared.ResultTypes;
using System.Text.Json;
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
                var response = Response<string>.Fail("Token yoxdu, get ozuve token al!", StatusCodes.Status401Unauthorized);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }
            await next(context);
        }
    }
}
