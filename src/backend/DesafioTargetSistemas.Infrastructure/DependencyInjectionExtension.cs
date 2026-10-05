using DesafioTargetSistemas.Infrastructure.DataAccess;
using DesafioTargetSistemas.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioTargetSistemas.Infrastructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
        }

        private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();

            services.AddDbContext<DesafioTargetSistemasDbContext>(dbContextOption =>
            {
                dbContextOption.UseSqlServer(connectionString);
            });
        }
    }
}
