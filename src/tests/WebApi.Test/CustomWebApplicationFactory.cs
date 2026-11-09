using CommonTestUtilities.Entities;
using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Security.Tokens;
using DesafioTargetSistemas.Domain.Services;
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
    private Product _product = default!;
    private Seller _seller = default!;
    private AppUser _user = default!;
    private string _password = string.Empty;
    private List<Sale> _sales = [];
    private List<AccountReceivable> _accountsReceivable = [];

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

    public decimal GetProductPrice() => _product.Price;

    public string GetProductDescription() => _product.Description;

    public int GetSellerId() => _seller.Id;

    public string GetUserEmail() => _user.Email;

    public string GetUserPassword() => _password;

    public string GetSellerName() => _seller.Name;

    public IReadOnlyList<Sale> GetSales() => _sales;

    public IReadOnlyList<AccountReceivable> GetAccountsReceivable() => _accountsReceivable;

    public AccountReceivable GetAccountReceivableFromDatabase(int id)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DesafioTargetSistemasDbContext>();

        return context.AccountReceivables.AsNoTracking().Single(a => a.Id == id);
    }

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
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        _product = ProductBuilder.Build();
        _seller = SellerBuilder.Build();

        _sales =
        [
            SaleBuilder.Build(_seller, new DateTime(2026, 9, 15, 10, 0, 0, DateTimeKind.Utc), 1000.00m),
            SaleBuilder.Build(_seller, new DateTime(2026, 10, 20, 15, 30, 0, DateTimeKind.Utc), 300.00m)
        ];

        var overdue = AccountReceivableBuilder.Build(today.AddDays(-4));
        var dueToday = AccountReceivableBuilder.Build(today);
        var future = AccountReceivableBuilder.Build(today.AddDays(10));

        var paid = AccountReceivableBuilder.Build(today.AddDays(-8));
        paid.PaymentDate = today.AddDays(-2);
        paid.PaidAmount = paid.Amount + InterestCalculator.Calculate(paid.Amount, InterestCalculator.DaysOverdue(paid.DueDate, paid.PaymentDate.Value));

        _accountsReceivable = [overdue, dueToday, future, paid];

        foreach (var account in _accountsReceivable)
            account.Sale = _sales[0];

        _password = "SenhaDeTeste@123";
        _user = AppUserBuilder.Build();
        _user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(_password);

        context.Products.Add(_product);
        context.Sellers.Add(_seller);
        context.Sales.AddRange(_sales);
        context.AccountReceivables.AddRange(_accountsReceivable);
        context.AppUsers.Add(_user);
        context.SaveChanges();
    }
}
