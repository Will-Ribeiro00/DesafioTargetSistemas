using Bogus;
using DesafioTargetSistemas.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class SellerBuilder
{
    public static Seller Build()
    {
        return new Faker<Seller>()
            .RuleFor(s => s.Id, _ => 1)
            .RuleFor(s => s.Name, f => f.Person.FullName);
    }
}
