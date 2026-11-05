using Bogus;
using DesafioTargetSistemas.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class AppUserBuilder
{
    public static AppUser Build()
    {
        return new Faker<AppUser>()
            .RuleFor(u => u.Id, _ => 1)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => f.Random.Hash())
            .RuleFor(u => u.UserType, _ => "ADMIN");
    }
}
