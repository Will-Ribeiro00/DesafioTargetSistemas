using Bogus;
using DesafioTargetSistemas.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class ProductBuilder
{
    public static Product Build()
    {
        return new Faker<Product>()
            .RuleFor(p => p.Id, _ => 1)
            .RuleFor(p => p.Code, f => f.Random.Int(100, 999).ToString())
            .RuleFor(p => p.Description, f => f.Commerce.ProductName())
            .RuleFor(p => p.Price, f => Math.Round(f.Random.Decimal(1, 100), 2))
            .RuleFor(p => p.CurrentStock, f => f.Random.Int(50, 200));
    }
}
