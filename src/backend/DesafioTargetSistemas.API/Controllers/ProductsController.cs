using DesafioTargetSistemas.Application.UseCases.Products.GetAll;
using DesafioTargetSistemas.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DesafioTargetSistemas.API.Controllers
{
    public class ProductsController : DesafioTargetSistemasBaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseProductJson>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromServices] IGetProductsUseCase useCase)
        {
            var response = await useCase.Execute();

            return Ok(response);
        }
    }
}
