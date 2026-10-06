using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Domain.Security.Cryptography;
using DesafioTargetSistemas.Domain.Security.Tokens;
using DesafioTargetSistemas.Infrastructure.DataAccess;
using DesafioTargetSistemas.Infrastructure.DataAccess.Repositories;
using DesafioTargetSistemas.Infrastructure.Extensions;
using DesafioTargetSistemas.Infrastructure.Security.Cryptography;
using DesafioTargetSistemas.Infrastructure.Security.Tokens;
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
            AddPasswordEncripter(services);
            AddTokens(services, configuration);
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

            services.AddScoped<ISellerRepository, SellerRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();

            services.AddScoped<IAccountReceivableRepository, AccountReceivableRepository>();

            services.AddScoped<IAppUserRepository, AppUserRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        private static void AddPasswordEncripter(IServiceCollection services)
        {
            services.AddScoped<IPasswordEncripter, BCryptPasswordEncripter>();
        }
        private static void AddTokens(IServiceCollection services, IConfiguration configuration)
        {
            var expirationTimeMinutes = configuration.JwtExpirationTimeMinutes();
            var signingKey = configuration.JwtSigningKey();

            services.AddScoped<IAccessTokenGenerator>(_ => new JwtTokenGenerator(expirationTimeMinutes, signingKey));
            services.AddScoped<IAccessTokenValidator>(_ => new JwtTokenValidator(signingKey));
        }
    }
}
