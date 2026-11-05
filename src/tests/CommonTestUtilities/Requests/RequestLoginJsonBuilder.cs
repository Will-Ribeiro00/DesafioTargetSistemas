using Bogus;
using DesafioTargetSistemas.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestLoginJsonBuilder
{
    public static RequestLoginJson Build()
    {
        return new Faker<RequestLoginJson>()
            .RuleFor(r => r.Email, f => f.Internet.Email())
            .RuleFor(r => r.Password, f => f.Internet.Password());
    }
}
