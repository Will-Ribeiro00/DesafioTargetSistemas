using System.Net;

namespace DesafioTargetSistemas.Exception.ExceptionsBase
{
    public abstract class DesafioTargetSistemasException(string message) : SystemException(message)
    {
        public abstract IList<string> GetErrorMessages();
        public abstract HttpStatusCode GetStatusCode();
    }
}
