using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Security.Tokens;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace DesafioTargetSistemas.API.Filters
{
    public class AuthenticatedRequestFilter(IAccessTokenValidator tokenValidator) : IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var token = ExtractBearerToken(context);
                var userId = tokenValidator.ValidateAndGetUserId(token);

                if (userId == 0)
                    throw new UnauthorizedException(ResourceMessageException.TOKEN_WITHOUT_USER);
            }
            catch (SecurityTokenExpiredException)
            {
                context.Result = new UnauthorizedObjectResult(
                    new ResponseErrorJson(ResourceMessageException.TOKEN_EXPIRED) { TokenIsExpired = true });
            }
            catch (DesafioTargetSistemasException ex)
            {
                context.Result = new ObjectResult(new ResponseErrorJson(ex.GetErrorMessages()))
                {
                    StatusCode = (int)ex.GetStatusCode()
                };
            }
            catch
            {
                context.Result = new UnauthorizedObjectResult(
                    new ResponseErrorJson(ResourceMessageException.INVALID_TOKEN));
            }

            return Task.CompletedTask;
        }

        private static string ExtractBearerToken(AuthorizationFilterContext context)
        {
            var authorization = context.HttpContext.Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authorization))
                throw new UnauthorizedException(ResourceMessageException.WITHOUT_TOKEN);

            if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedException(ResourceMessageException.INVALID_TOKEN);

            return authorization["Bearer ".Length..].Trim();
        }
    }
}
