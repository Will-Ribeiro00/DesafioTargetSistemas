using Bogus;
using DesafioTargetSistemas.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestSellerCommissionsJsonBuilder
{
    public static RequestSellerCommissionsJson Build()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return new Faker<RequestSellerCommissionsJson>()
            .RuleFor(r => r.To, _ => today)
            .RuleFor(r => r.From, f => today.AddDays(-f.Random.Int(0, 60)));
    }
}
