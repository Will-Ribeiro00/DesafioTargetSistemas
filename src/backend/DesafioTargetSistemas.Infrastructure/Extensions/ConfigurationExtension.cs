using Microsoft.Extensions.Configuration;

namespace DesafioTargetSistemas.Infrastructure.Extensions
{
    public static class ConfigurationExtension
    {
        public static string ConnectionString(this IConfiguration configuration)
        {
            return configuration.GetConnectionString("DefaultConnectionSqlServer")!;
        }

        public static string JwtSigningKey(this IConfiguration configuration)
        {
            return configuration["Settings:Jwt:SigningKey"]!;
        }

        public static uint JwtExpirationTimeMinutes(this IConfiguration configuration)
        {
            return configuration.GetValue<uint>("Settings:Jwt:ExpirationTimeMinutes");
        }
    }
}
