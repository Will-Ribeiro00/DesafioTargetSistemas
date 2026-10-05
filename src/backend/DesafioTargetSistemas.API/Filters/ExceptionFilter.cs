using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DesafioTargetSistemas.API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DesafioTargetSistemasException exception)
                HandleProjectException(exception, context);
            else
                ThrowUnknownException(context);
        }

        private static void HandleProjectException(DesafioTargetSistemasException exception, ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)exception.GetStatusCode();
            context.Result = new ObjectResult(new ResponseErrorJson(exception.GetErrorMessages()));
        }

        private static void ThrowUnknownException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessageException.UNKNOWN_ERROR));
        }
    }
}
