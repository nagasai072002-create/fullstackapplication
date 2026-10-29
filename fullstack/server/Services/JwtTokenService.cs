using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using fullstackapplication.Server.Models;
using Microsoft.IdentityModel.Tokens;

namespace fullstackapplication.Server.Services;

public sealed class JwtTokenService(byte[] signingKey, string issuer, string audience, int lifetimeMinutes)
{
    public (string AccessToken, DateTime ExpiresAtUtc) CreateToken(EmployeeAccount account)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(lifetimeMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, account.Username),
            new Claim("role", account.Role)
        };
        var credentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, audience, claims, expires: expiresAtUtc, signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
