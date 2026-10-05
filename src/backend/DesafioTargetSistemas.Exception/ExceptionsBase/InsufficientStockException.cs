using System.Net;

namespace DesafioTargetSistemas.Exception.ExceptionsBase
{
    public class InsufficientStockException : DesafioTargetSistemasException
    {
        public InsufficientStockException() : base(ResourceMessageException.INSUFFICIENT_STOCK) { }

        public override IList<string> GetErrorMessages() => [Message];
        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
    }
}
