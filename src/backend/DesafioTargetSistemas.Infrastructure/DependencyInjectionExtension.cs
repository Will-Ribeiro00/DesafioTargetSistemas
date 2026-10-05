using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Infrastructure.DataAccess;
using DesafioTargetSistemas.Infrastructure.DataAccess.Repositories;
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
            AddRepositories(services);
        }

        private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();

            services.AddDbContext<DesafioTargetSistemasDbContext>(dbContextOption =>
            {
                dbContextOption.UseSqlServer(connectionString);
            });
        }
        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
