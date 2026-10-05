using DesafioTargetSistemas.Application.UseCases.AccountsReceivable.GetOpen;
using DesafioTargetSistemas.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DesafioTargetSistemas.API.Controllers
{
    public class AccountsReceivableController : DesafioTargetSistemasBaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseAccountReceivableJson>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOpen([FromServices] IGetAccountsReceivableUseCase useCase)
        {
            var response = await useCase.Execute();

            return Ok(response);
        }
    }
}
