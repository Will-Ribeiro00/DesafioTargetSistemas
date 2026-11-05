using DesafioTargetSistemas.Domain.Security.Cryptography;
using Moq;

namespace CommonTestUtilities.Cryptography;

public class PasswordEncripterBuilder
{
    private readonly Mock<IPasswordEncripter> _encripter;

    public PasswordEncripterBuilder() => _encripter = new Mock<IPasswordEncripter>();

    public IPasswordEncripter Build() => _encripter.Object;

    public void IsValid(string password, string passwordHash)
    {
        _encripter.Setup(encripter => encripter.IsValid(password, passwordHash)).Returns(true);
    }
}
