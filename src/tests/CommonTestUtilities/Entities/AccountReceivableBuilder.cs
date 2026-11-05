using Bogus;
using DesafioTargetSistemas.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class AccountReceivableBuilder
{
    public static AccountReceivable Build(DateOnly dueDate)
    {
        return new Faker<AccountReceivable>()
            .RuleFor(a => a.SaleId, f => f.Random.Int(1, 100))
            .RuleFor(a => a.Amount, f => Math.Round(f.Random.Decimal(10, 2000), 2))
            .RuleFor(a => a.DueDate, _ => dueDate)
            .RuleFor(a => a.PaymentDate, _ => null);
    }
}