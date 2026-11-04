using Bogus;
using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Domain.Enums;

namespace CommonTestUtilities.Requests;

public class RequestRegisterStockMovementJsonBuilder
{
    public static RequestRegisterStockMovementJson Build()
    {
        return new Faker<RequestRegisterStockMovementJson>()
            .RuleFor(r => r.Code, f => f.Random.Int(100, 999).ToString())
            .RuleFor(r => r.Type, f => f.PickRandom<StockMovementType>())
            .RuleFor(r => r.Description, f => f.Commerce.ProductName())
            .RuleFor(r => r.Quantity, f => f.Random.Int(1, 100));
    }
}
