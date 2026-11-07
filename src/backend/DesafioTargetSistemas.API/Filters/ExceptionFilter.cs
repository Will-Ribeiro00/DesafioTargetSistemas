using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DesafioTargetSistemas.API.Filters
{
    public class ExceptionFilter(ILogger<ExceptionFilter> logger) : IExceptionFilter
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

        private void ThrowUnknownException(ExceptionContext context)
        {
            logger.LogError(context.Exception,
                            "Unhandled exception on {Method} {Path}",
                            context.HttpContext.Request.Method,
                            context.HttpContext.Request.Path);

            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessageException.UNKNOWN_ERROR));
        }
    }
}
