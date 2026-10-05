using DesafioTargetSistemas.Application.UseCases.Sellers.GetCommissions;
using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DesafioTargetSistemas.API.Controllers
{
    public class SellersController : DesafioTargetSistemasBaseController
    {
        [HttpGet("commissions")]
        [ProducesResponseType(typeof(List<ResponseSellerCommissionJson>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCommissions([FromServices] IGetSellerCommissionsUseCase useCase,
                                                        [FromQuery] RequestSellerCommissionsJson request)
        {
            var response = await useCase.Execute(request);

            return Ok(response);
        }
    }
}
