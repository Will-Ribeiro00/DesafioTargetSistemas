using System.Net;

namespace DesafioTargetSistemas.Exception.ExceptionsBase
{
    public class ErrorOnValidationException(IList<string> errorMessages) : DesafioTargetSistemasException(string.Empty)
    {
        private readonly IList<string> _errorMessages = errorMessages;

        public override IList<string> GetErrorMessages() => _errorMessages;
        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
    }
}
