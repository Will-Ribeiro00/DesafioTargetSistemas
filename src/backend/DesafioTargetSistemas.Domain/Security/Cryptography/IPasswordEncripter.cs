namespace DesafioTargetSistemas.Domain.Security.Cryptography
{
    public interface IPasswordEncripter
    {
        bool IsValid(string password, string passwordHash);
    }
}
