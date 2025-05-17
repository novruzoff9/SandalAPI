using IdentityServer.DTOs;
using IdentityServer.Helpers;
using System.Security.Claims;

namespace IdentityServer.Services;

public interface IAuthenticationService
{
    Task<TokenResponseDto> LoginAsync(string email, string password);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(IUserService userService, JwtTokenGenerator jwtTokenGenerator)
    {
        _userService = userService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<TokenResponseDto> LoginAsync(string email, string password)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        var pass = BCrypt.Net.BCrypt.Verify(password, user.HashedPassword);
        if (!pass)
        {
            throw new UnauthorizedAccessException("Şifrə yanlışdır");
        }
        List<Claim> claims = new List<Claim>
        {
            new Claim("company", user.CompanyId),
            new Claim("warehouse", user.WarehouseId)
        };
        var token = _jwtTokenGenerator.GenerateToken(user, claims);
        return token;
    }
}
