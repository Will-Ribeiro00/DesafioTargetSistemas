using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using DesafioTargetSistemas.Application.UseCases.Login.DoLogin;
using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;
using Shouldly;

namespace UseCase.Test.Login.DoLogin;

public class DoLoginUseCaseTest
{
    private const string TOKEN = "token-de-teste";

    [Fact]
    public async Task Success()
    {
        // Arrange
        var user = AppUserBuilder.Build();
        const string password = "SenhaCorreta@123";

        var request = RequestLoginJsonBuilder.Build();
        request.Email = user.Email;
        request.Password = password;

        var useCase = CreateUseCase(user, password);

        // Act
        var result = await useCase.Execute(request);

        // Assert
        result.ShouldNotBeNull();
        result.AccessToken.ShouldBe(TOKEN);
    }

    [Fact]
    public async Task ErrorUserNotFound()
    {
        // Arrange
        var request = RequestLoginJsonBuilder.Build();
        var useCase = CreateUseCase();

        // Act
        Func<Task> act = async () => await useCase.Execute(request);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidLoginException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.INVALID_LOGIN);
    }

    [Fact]
    public async Task ErrorWrongPassword()
    {
        // Arrange
        var user = AppUserBuilder.Build();

        var request = RequestLoginJsonBuilder.Build();
        request.Email = user.Email;
        request.Password = "SenhaErrada@123";

        var useCase = CreateUseCase(user, password: "SenhaCorreta@123");

        // Act
        Func<Task> act = async () => await useCase.Execute(request);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidLoginException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.INVALID_LOGIN);
    }

    [Fact]
    public async Task ErrorInvalidRequest()
    {
        // Arrange
        var request = RequestLoginJsonBuilder.Build();
        request.Email = string.Empty;

        var useCase = CreateUseCase();

        // Act
        Func<Task> act = async () => await useCase.Execute(request);

        // Assert
        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.EMAIL_REQUIRED);
    }

    private static DoLoginUseCase CreateUseCase(AppUser? user = null, string? password = null)
    {
        var userRepository = new AppUserRepositoryBuilder();
        var passwordEncripter = new PasswordEncripterBuilder();

        if (user is not null)
            userRepository.GetByEmail(user);

        if (user is not null && password is not null)
            passwordEncripter.IsValid(password, user.PasswordHash);

        return new DoLoginUseCase(
            userRepository.Build(),
            passwordEncripter.Build(),
            AccessTokenGeneratorBuilder.Build(TOKEN));
    }
}