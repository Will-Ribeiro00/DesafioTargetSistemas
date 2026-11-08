using DesafioTargetSistemas.Application.UseCases.AccountsReceivable.GetOpen;
using DesafioTargetSistemas.Application.UseCases.AccountsReceivable.Pay;
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

        [HttpPatch("{id:int}/payment")]
        [ProducesResponseType(typeof(ResponseAccountReceivableJson), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Pay([FromServices] IPayAccountReceivableUseCase useCase,
                                             [FromRoute] int id)
        {
            var response = await useCase.Execute(id);

            return Ok(response);
        } 
    }
}
