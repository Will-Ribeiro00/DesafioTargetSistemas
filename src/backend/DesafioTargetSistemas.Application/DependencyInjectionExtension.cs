using DesafioTargetSistemas.Application.UseCases.AccountsReceivable.GetOpen;
using DesafioTargetSistemas.Application.UseCases.Login.DoLogin;
using DesafioTargetSistemas.Application.UseCases.Products.GetAll;
using DesafioTargetSistemas.Application.UseCases.Sales.GetAll;
using DesafioTargetSistemas.Application.UseCases.Sales.Register;
using DesafioTargetSistemas.Application.UseCases.Sellers.GetAll;
using DesafioTargetSistemas.Application.UseCases.Sellers.GetCommissions;
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
            services.AddScoped<IGetProductsUseCase, GetProductsUseCase>();
            services.AddScoped<IGetSalesUseCase, GetSalesUseCase>();
            services.AddScoped<IGetSellerCommissionsUseCase, GetSellerCommissionsUseCase>();
            services.AddScoped<IGetSellersUseCase, GetSellersUseCase>();
            services.AddScoped<IGetAccountsReceivableUseCase, GetAccountsReceivableUseCase>();
            services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();
        }
    }
}
