using DesafioTargetSistemas.Domain.Entities;

namespace DesafioTargetSistemas.Domain.Repositories
{
    public interface IAccountReceivableRepository
    {
        Task<List<AccountReceivable>> GetOpen();
        Task<AccountReceivable?> GetById(int id);
        void Update(AccountReceivable accountReceivable);
    }
}
