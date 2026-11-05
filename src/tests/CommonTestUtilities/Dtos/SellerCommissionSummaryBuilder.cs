using Bogus;
using DesafioTargetSistemas.Domain.Dtos;

namespace CommonTestUtilities.Dtos;

public class SellerCommissionSummaryBuilder
{
    public static List<SellerCommissionSummary> Collection(int count = 2)
    {
        var faker = new Faker();

        return [.. Enumerable.Range(1, count)
            .Select(index => new SellerCommissionSummary(
                SellerId: index,
                SellerName: faker.Person.FullName,
                SalesCount: faker.Random.Int(1, 10),
                TotalSold: Math.Round(faker.Random.Decimal(100, 5000), 2),
                TotalCommission: Math.Round(faker.Random.Decimal(0, 250), 2))
            )];
    }
}