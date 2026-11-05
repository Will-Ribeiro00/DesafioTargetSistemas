using Bogus;
using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Domain.ValueObjects;

namespace CommonTestUtilities.Requests;

public class RequestRegisterSaleJsonBuilder
{
    public static RequestRegisterSaleJson Build(int itemsCount = 2)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return new Faker<RequestRegisterSaleJson>()
            .RuleFor(r => r.SellerId, f => f.Random.Int(1, 100))
            .RuleFor(r => r.DueDate, f => today.AddDays(f.Random.Int(0, DesafioTargetSistemasRuleConstants.SALE_MAX_DUE_DAYS)))
            .RuleFor(r => r.Items, f => [.. Enumerable.Range(1, itemsCount)
                .Select(index => new RequestSaleItemJson
                {
                    Code = (100 + index).ToString(),
                    Quantity = f.Random.Int(1, 10)
                })]);
    }
}
