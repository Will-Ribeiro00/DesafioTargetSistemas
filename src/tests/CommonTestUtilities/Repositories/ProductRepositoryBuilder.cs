using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Moq;

namespace CommonTestUtilities.Repositories;

public class ProductRepositoryBuilder
{
    private readonly Mock<IProductRepository> _repository;

    public ProductRepositoryBuilder() => _repository = new Mock<IProductRepository>();

    public IProductRepository Build() => _repository.Object;

    public void GetByCode(Product product)
    {
        _repository.Setup(repository => repository.GetByCode(product.Code)).ReturnsAsync(product);
    }

    public void GetAll(List<Product> products)
    {
        _repository.Setup(repository => repository.GetAll()).ReturnsAsync(products);
    }
}
