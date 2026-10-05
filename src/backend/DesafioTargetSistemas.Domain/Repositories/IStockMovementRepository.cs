using DesafioTargetSistemas.Domain.Entities;

namespace DesafioTargetSistemas.Domain.Repositories
{
    public interface IStockMovementRepository
    {
        Task Add(StockMovement movement);
    }
}
