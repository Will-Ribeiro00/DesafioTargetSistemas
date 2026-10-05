using DesafioTargetSistemas.Application.UseCases.Sales.GetAll;
using DesafioTargetSistemas.Application.UseCases.Sales.Register;
using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DesafioTargetSistemas.API.Controllers
{
    public class SalesController : DesafioTargetSistemasBaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredSaleJson), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Register([FromServices] IRegisterSaleUseCase useCase,
                                                  [FromBody] RequestRegisterSaleJson request)
        {
            var response = await useCase.Execute(request);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseSaleJson>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromServices] IGetSalesUseCase useCase)
        {
            var response = await useCase.Execute();

            return Ok(response);
        }
    }
}
