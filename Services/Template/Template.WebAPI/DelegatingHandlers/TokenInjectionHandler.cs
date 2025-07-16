
namespace Template.WebAPI.DelegatingHandlers;

public class TokenInjectionHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenInjectionHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(token))
        {
            // Remove "Bearer " prefix if it exists
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }
            // Add the token to the request headers
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return base.SendAsync(request, cancellationToken);
    }
}
