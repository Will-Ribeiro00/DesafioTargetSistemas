using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Moq;

namespace CommonTestUtilities.Repositories;

public class AccountReceivableRepositoryBuilder
{
    private readonly Mock<IAccountReceivableRepository> _repository;

    public AccountReceivableRepositoryBuilder() => _repository = new Mock<IAccountReceivableRepository>();

    public IAccountReceivableRepository Build() => _repository.Object;

    public void GetOpen(List<AccountReceivable> accounts)
    {
        _repository.Setup(repository => repository.GetOpen()).ReturnsAsync(accounts);
    }
}