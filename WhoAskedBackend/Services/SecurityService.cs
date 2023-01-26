using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WhoAskedBackend.Model;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace WhoAskedBackend.Services;

public class SecurityService
{
    private const int TokenExpirationInSeconds = 600;
    private readonly IConfiguration _configuration;

    public SecurityService([FromServices] IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string HashPassword(string? password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    public bool VerifyPassword(string? password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }

    public string BuildJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var roles = new List<string> {user.Role};
        var roleClaims = roles.ToDictionary(
            q => ClaimTypes.Role,
            q => (object) q.ToUpper());

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"] ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"] ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddSeconds(TokenExpirationInSeconds),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature),
            Claims = roleClaims
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var ret = tokenHandler.WriteToken(token);

        return ret;
    }
}