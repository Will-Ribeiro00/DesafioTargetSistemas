using DesafioTargetSistemas.Application.UseCases.Customers.Register;
using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DesafioTargetSistemas.API.Controllers;

public class CustomersController : DesafioTargetSistemasBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisteredCustomerJson), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromServices] IRegisterCustomerUseCase useCase,
                                              [FromBody] RequestRegisterCustomerJson request)
    {
        var response = await useCase.Execute(request);

        return Created(string.Empty, response);
    }
}
