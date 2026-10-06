using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Domain.Security.Cryptography;
using DesafioTargetSistemas.Domain.Security.Tokens;
using DesafioTargetSistemas.Exception.ExceptionsBase;

namespace DesafioTargetSistemas.Application.UseCases.Login.DoLogin
{
    public class DoLoginUseCase : IDoLoginUseCase
    {
        private readonly IAppUserRepository _userRepository;
        private readonly IPasswordEncripter _passwordEncripter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;

        public DoLoginUseCase(
            IAppUserRepository userRepository,
            IPasswordEncripter passwordEncripter,
            IAccessTokenGenerator accessTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordEncripter = passwordEncripter;
            _accessTokenGenerator = accessTokenGenerator;
        }

        public async Task<ResponseLoginJson> Execute(RequestLoginJson request)
        {
            Validate(request);

            var user = await _userRepository.GetByEmail(request.Email);

            if (user is null || !_passwordEncripter.IsValid(request.Password, user.PasswordHash))
                throw new InvalidLoginException();

            return new ResponseLoginJson
            {
                AccessToken = _accessTokenGenerator.Generate(user)
            };
        }

        private static void Validate(RequestLoginJson request)
        {
            var result = new DoLoginValidator().Validate(request);

            if (!result.IsValid)
                throw new ErrorOnValidationException([.. result.Errors.Select(e => e.ErrorMessage)]);
        }
    }
}
