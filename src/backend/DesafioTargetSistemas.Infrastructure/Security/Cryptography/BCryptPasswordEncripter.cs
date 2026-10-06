using DesafioTargetSistemas.Domain.Security.Cryptography;

namespace DesafioTargetSistemas.Infrastructure.Security.Cryptography
{
    internal class BCryptPasswordEncripter : IPasswordEncripter
    {
        public bool IsValid(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
