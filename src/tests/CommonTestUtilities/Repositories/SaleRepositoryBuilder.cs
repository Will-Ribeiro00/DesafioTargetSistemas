using DesafioTargetSistemas.Domain.Dtos;
using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Moq;

namespace CommonTestUtilities.Repositories;

public class SaleRepositoryBuilder
{
    private readonly Mock<ISaleRepository> _repository;

    public Sale? AddedSale { get; private set; }
    public DateTime? ReceivedFrom { get; private set; }
    public DateTime? ReceivedToExclusive { get; private set; }

    public SaleRepositoryBuilder()
    {
        _repository = new Mock<ISaleRepository>();

        _repository
            .Setup(repository => repository.Add(It.IsAny<Sale>()))
            .Callback<Sale>(sale => AddedSale = sale)
            .Returns(Task.CompletedTask);
    }

    public void GetCommissionsBySeller(List<SellerCommissionSummary> summaries)
    {
        _repository
            .Setup(repository => repository.GetCommissionsBySeller(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Callback<DateTime?, DateTime?>((from, toExclusive) =>
            {
                ReceivedFrom = from;
                ReceivedToExclusive = toExclusive;
            })
            .ReturnsAsync(summaries);
    }

    public void GetAll(List<Sale> sales)
    {
        _repository.Setup(repository => repository.GetAll()).ReturnsAsync(sales);
    }

    public ISaleRepository Build() => _repository.Object;
}
