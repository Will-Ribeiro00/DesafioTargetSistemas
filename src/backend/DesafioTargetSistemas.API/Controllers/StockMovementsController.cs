using DesafioTargetSistemas.Application.UseCases.StockMovements.Register;
using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DesafioTargetSistemas.API.Controllers
{
    public class StockMovementsController : DesafioTargetSistemasBaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredStockMovementJson), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Register([FromServices] IRegisterStockMovementUseCase useCase,
                                                  [FromBody] RequestRegisterStockMovementJson request)
        {
            var response = await useCase.Execute(request);

            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}
