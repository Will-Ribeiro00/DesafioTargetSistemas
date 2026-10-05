using DesafioTargetSistemas.Domain.Entities;

namespace DesafioTargetSistemas.Domain.Repositories
{
    public interface IAccountReceivableRepository
    {
        Task<List<AccountReceivable>> GetOpen();
    }
}
