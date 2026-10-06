using DesafioTargetSistemas.Domain.Entities;

namespace DesafioTargetSistemas.Domain.Repositories
{
    public interface IAppUserRepository
    {
        Task<AppUser?> GetByEmail(string email);
    }
}
