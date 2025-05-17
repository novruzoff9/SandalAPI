using FluentValidation;
using IdentityServer.Exceptions;
using System.Net;
using System.Text.Json;

namespace IdentityServer.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            FormatException _ => (int)HttpStatusCode.BadRequest,
            NotFoundException _ => (int)HttpStatusCode.NotFound,
            ConflictException _ => (int)HttpStatusCode.Conflict,
            UnauthorizedAccessException _ => (int)HttpStatusCode.Unauthorized,
            ValidationException _ => (int)HttpStatusCode.BadRequest, 
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            StatusCode = statusCode,
            Message = ex.Message,
            Detail = ex.InnerException?.Message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
