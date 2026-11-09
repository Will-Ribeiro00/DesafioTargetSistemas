using System.Net;

namespace DesafioTargetSistemas.Exception.ExceptionsBase
{
    public class ConcurrencyConflictException() : DesafioTargetSistemasException(ResourceMessageException.CONCURRENCY_CONFLICT)
    {
        public override IList<string> GetErrorMessages() => [Message];
        public override HttpStatusCode GetStatusCode() => HttpStatusCode.Conflict;
    }
}