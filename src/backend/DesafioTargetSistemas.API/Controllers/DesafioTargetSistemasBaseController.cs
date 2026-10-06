using DesafioTargetSistemas.API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace DesafioTargetSistemas.API.Controllers
{
    [Route("api/hml/[controller]")]
    [ApiController]
    [AuthenticatedUser]
    public class DesafioTargetSistemasBaseController : ControllerBase
    {
    }
}
