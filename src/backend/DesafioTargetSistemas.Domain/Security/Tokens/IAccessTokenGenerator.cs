using DesafioTargetSistemas.Domain.Entities;

namespace DesafioTargetSistemas.Domain.Security.Tokens
{
    public interface IAccessTokenGenerator
    {
        string Generate(AppUser user);
    }
}
