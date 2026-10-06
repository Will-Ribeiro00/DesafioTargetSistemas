using DesafioTargetSistemas.Application.UseCases.Login.DoLogin;
using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DesafioTargetSistemas.API.Controllers
{
    [Route("api/hml/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseLoginJson), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromServices] IDoLoginUseCase useCase,
                                               [FromBody] RequestLoginJson request)
        {
            var response = await useCase.Execute(request);

            return Ok(response);
        }
    }
}
