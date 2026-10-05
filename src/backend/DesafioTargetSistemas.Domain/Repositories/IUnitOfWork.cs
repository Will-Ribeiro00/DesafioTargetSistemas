namespace DesafioTargetSistemas.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}
