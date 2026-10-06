using System.Net;

namespace DesafioTargetSistemas.Exception.ExceptionsBase
{
    public class UnauthorizedException(string message) : DesafioTargetSistemasException(message)
    {
        public override IList<string> GetErrorMessages() => [Message];
        public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
    }
}
