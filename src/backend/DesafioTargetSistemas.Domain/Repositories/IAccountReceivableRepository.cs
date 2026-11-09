using DesafioTargetSistemas.Domain.Entities;

namespace DesafioTargetSistemas.Domain.Repositories
{
    public interface IAccountReceivableRepository
    {
        Task<List<AccountReceivable>> GetOpen();
        Task<List<AccountReceivable>> GetPaid();
        Task<AccountReceivable?> GetById(int id);
        void Update(AccountReceivable accountReceivable);
    }
}
