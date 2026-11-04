using CommonTestUtilities.Entities;
using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Security.Tokens;
using DesafioTargetSistemas.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebApi.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly AppUser _user = new() { Id = 1, Email = "admin@test.com", UserType = "ADMIN" };
    private Product _product = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test")
            .ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<DesafioTargetSistemasDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<DesafioTargetSistemasDbContext>>();

                services.AddDbContext<DesafioTargetSistemasDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));

                using var scope = services.BuildServiceProvider().CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DesafioTargetSistemasDbContext>();

                context.Database.EnsureDeleted();
                StartDatabase(context);
            });
    }

    public string GetProductCode() => _product.Code;

    public int GetProductCurrentStock()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DesafioTargetSistemasDbContext>();

        return context.Products.AsNoTracking().Single(p => p.Code == _product.Code).CurrentStock;
    }

    public string GetToken()
    {
        using var scope = Services.CreateScope();
        var tokenGenerator = scope.ServiceProvider.GetRequiredService<IAccessTokenGenerator>();

        return tokenGenerator.Generate(_user);
    }

    private void StartDatabase(DesafioTargetSistemasDbContext context)
    {
        _product = ProductBuilder.Build();

        context.Products.Add(_product);
        context.SaveChanges();
    }
}
