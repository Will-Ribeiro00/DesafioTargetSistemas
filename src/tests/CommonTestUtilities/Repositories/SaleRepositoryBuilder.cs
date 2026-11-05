using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Moq;

namespace CommonTestUtilities.Repositories;

public class SaleRepositoryBuilder
{
    private readonly Mock<ISaleRepository> _repository;

    public Sale? AddedSale { get; private set; }

    public SaleRepositoryBuilder()
    {
        _repository = new Mock<ISaleRepository>();

        _repository
            .Setup(repository => repository.Add(It.IsAny<Sale>()))
            .Callback<Sale>(sale => AddedSale = sale)
            .Returns(Task.CompletedTask);
    }

    public ISaleRepository Build() => _repository.Object;
}
