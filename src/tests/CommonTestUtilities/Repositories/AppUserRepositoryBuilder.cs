using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Moq;

namespace CommonTestUtilities.Repositories;

public class AppUserRepositoryBuilder
{
    private readonly Mock<IAppUserRepository> _repository;

    public AppUserRepositoryBuilder() => _repository = new Mock<IAppUserRepository>();

    public IAppUserRepository Build() => _repository.Object;

    public void GetByEmail(AppUser user)
    {
        _repository.Setup(repository => repository.GetByEmail(user.Email)).ReturnsAsync(user);
    }
}
