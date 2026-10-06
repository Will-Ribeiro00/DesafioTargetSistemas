namespace DesafioTargetSistemas.Domain.Security.Tokens
{
    public interface IAccessTokenValidator
    {
        int ValidateAndGetUserId(string token);
    }
}
