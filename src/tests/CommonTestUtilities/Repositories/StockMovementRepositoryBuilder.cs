using DesafioTargetSistemas.Domain.Repositories;
using Moq;

namespace CommonTestUtilities.Repositories;

public class StockMovementRepositoryBuilder
{
    public static IStockMovementRepository Build() => new Mock<IStockMovementRepository>().Object;
}
