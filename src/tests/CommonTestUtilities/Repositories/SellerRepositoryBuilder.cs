using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Moq;

namespace CommonTestUtilities.Repositories;

public class SellerRepositoryBuilder
{
    private readonly Mock<ISellerRepository> _repository;

    public SellerRepositoryBuilder() => _repository = new Mock<ISellerRepository>();

    public ISellerRepository Build() => _repository.Object;

    public void GetById(Seller seller)
    {
        _repository.Setup(repository => repository.GetById(seller.Id)).ReturnsAsync(seller);
    }

    public void GetAll(List<Seller> sellers)
    {
        _repository.Setup(repository => repository.GetAll()).ReturnsAsync(sellers);
    }
}
