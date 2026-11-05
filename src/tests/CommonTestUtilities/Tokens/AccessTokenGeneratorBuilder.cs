using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Security.Tokens;
using Moq;

namespace CommonTestUtilities.Tokens;

public class AccessTokenGeneratorBuilder
{
    public static IAccessTokenGenerator Build(string token)
    {
        var generator = new Mock<IAccessTokenGenerator>();
        generator.Setup(g => g.Generate(It.IsAny<AppUser>())).Returns(token);

        return generator.Object;
    }
}
