using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.Login.DoLogin
{
    public interface IDoLoginUseCase
    {
        Task<ResponseLoginJson> Execute(RequestLoginJson request);
    }
}
