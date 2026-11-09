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

    public void GetPaid(List<AccountReceivable> accounts)
    {
        _repository.Setup(repository => repository.GetPaid()).ReturnsAsync(accounts);
    }

    public void GetById(AccountReceivable account)
    {
        _repository.Setup(repository => repository.GetById(account.Id)).ReturnsAsync(account);
    }
}