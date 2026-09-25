using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LiquorShop.API.Data.Entities;
using Microsoft.IdentityModel.Tokens;

namespace LiquorShop.API.Helpers;

public static class JwtHelper
{
    public static string GenerateToken(User user, JwtSettings settings)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name,               user.FullName),
            new Claim(ClaimTypes.Role,               user.Role?.Name ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             settings.Issuer,
            audience:           settings.Audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
