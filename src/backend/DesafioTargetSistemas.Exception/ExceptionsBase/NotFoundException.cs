using System.Net;

namespace DesafioTargetSistemas.Exception.ExceptionsBase
{
    public class NotFoundException(string message) : DesafioTargetSistemasException(message)
    {
        public override IList<string> GetErrorMessages() => [Message];
        public override HttpStatusCode GetStatusCode() => HttpStatusCode.NotFound;
    }
}
