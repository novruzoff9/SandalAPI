using Microsoft.AspNetCore.Http;
using Organization.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Organization.Application.Common.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = JsonSerializer.Serialize(new 
            { 
                errors = ex.Errors,  
            });
            await context.Response.WriteAsync(response);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = JsonSerializer.Serialize(new 
            { 
                message = ex.Message,
                stackTrace = ex.StackTrace
            });
            await context.Response.WriteAsync(response);
        }
    }
}
