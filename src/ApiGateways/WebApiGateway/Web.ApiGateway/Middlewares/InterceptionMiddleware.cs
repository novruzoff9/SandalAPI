namespace Web.ApiGateway.Middlewares;

public class InterceptionMiddleware
{
    private readonly RequestDelegate next;

    public InterceptionMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers["Referrer"] = "Sandal-Gateway";
        await next(context);
    }
}
