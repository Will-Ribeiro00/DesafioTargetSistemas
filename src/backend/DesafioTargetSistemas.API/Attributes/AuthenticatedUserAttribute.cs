using DesafioTargetSistemas.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DesafioTargetSistemas.API.Attributes
{
    public class AuthenticatedUserAttribute : TypeFilterAttribute
    {
        public AuthenticatedUserAttribute() : base(typeof(AuthenticatedRequestFilter)) { }
    }
}
