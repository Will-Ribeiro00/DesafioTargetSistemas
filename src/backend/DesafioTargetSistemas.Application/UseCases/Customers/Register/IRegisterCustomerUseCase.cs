using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.Customers.Register;

public interface IRegisterCustomerUseCase
{
    Task<ResponseRegisteredCustomerJson> Execute(RequestRegisterCustomerJson request);
}
