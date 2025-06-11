using IdentityServer.Configurations;
using IdentityServer.DTOs;
using IdentityServer.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityServer.Helpers;

public class JwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    public JwtTokenGenerator(IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value;
    }

    public TokenResponseDto GenerateToken(UserDetailedDto user, List<Claim>? extraClaims = null)
    {
        var claims = new List<Claim>
        {
            new Claim("sub", user.Id),
            new Claim("email", user.Email),
            new Claim("fullName", $"{user.FirstName} {user.LastName}")
        };
        if (extraClaims != null)
        {
            claims.AddRange(extraClaims);
        }

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim("roles", role.ToLower()));
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        TokenResponseDto tokenResponse = new TokenResponseDto
        {
            access_token = accessToken,
            expires_in = _jwtSettings.ExpiryMinutes * 60 * 60,
            token_type = "Bearer",
            refresh_token = string.Empty,
            scope = string.Empty
        };
        return tokenResponse;
    }
}
