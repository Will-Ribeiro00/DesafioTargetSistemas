using DesafioTargetSistemas.Application.UseCases.Sales.Register;
using DesafioTargetSistemas.Application.UseCases.StockMovements.Register;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioTargetSistemas.Application
{
    public static class DependencyInjectionExtension
    {
        public static void AddApplication(this IServiceCollection services)
        {
            AddUseCases(services);
            
        }

        private static void AddUseCases(IServiceCollection services)
        {
            services.AddScoped<IRegisterStockMovementUseCase, RegisterStockMovementUseCase>();
            services.AddScoped<IRegisterSaleUseCase, RegisterSaleUseCase>();
        }
    }
}
