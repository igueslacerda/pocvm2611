using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using pocservicedn.Contracts;
using pocservicedn.Entities;

namespace pocservicedn.Services;

public sealed class JwtTokenService(
    string issuer,
    string audience,
    SymmetricSecurityKey signingKey,
    int lifetimeMinutes) : IJwtTokenService
{
    public LoginResponse CreateToken(AcUsuarioEntity usuario)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(lifetimeMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Login),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), "Bearer", expiresAt);
    }
}
