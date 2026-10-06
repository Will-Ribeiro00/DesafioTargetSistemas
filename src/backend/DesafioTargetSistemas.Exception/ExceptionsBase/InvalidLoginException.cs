using System.Net;

namespace DesafioTargetSistemas.Exception.ExceptionsBase
{
    public class InvalidLoginException : DesafioTargetSistemasException
    {
        public InvalidLoginException() : base(ResourceMessageException.INVALID_LOGIN) { }

        public override IList<string> GetErrorMessages() => [Message];
        public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
    }
}
