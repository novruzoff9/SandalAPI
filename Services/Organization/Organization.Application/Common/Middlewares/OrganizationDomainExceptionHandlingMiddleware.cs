using Microsoft.AspNetCore.Http;
using Organization.Application.Common.Exceptions;
using Shared.ResultTypes;
using System.Net;
using System.Text.Json;

namespace Organization.Application.Common.Middlewares;

public class OrganizationDomainExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public OrganizationDomainExceptionHandlingMiddleware(RequestDelegate next)
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
            int statuscode = (int)HttpStatusCode.BadRequest;
            await HandleExceptionAsync(context, ex.Message, statuscode);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
    {
        var response = Response<string>.Fail(message, statusCode);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
