using DesafioTargetSistemas.Domain.Security.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DesafioTargetSistemas.Infrastructure.Security.Tokens
{
    internal class JwtTokenValidator(string signingKey) : IAccessTokenValidator
    {
        public int ValidateAndGetUserId(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userId, out var id) ? id : 0;
        }
    }
}
