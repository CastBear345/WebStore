using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Swagger.Helpers;

public static class JWTTokenHelpers
{
    public static string CreateTokenString(
        List<Claim> claims, DateTime expiration, SigningCredentials credentials)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}