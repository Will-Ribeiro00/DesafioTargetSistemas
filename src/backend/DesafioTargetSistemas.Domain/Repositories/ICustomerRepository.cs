using DesafioTargetSistemas.Domain.Entities;

namespace DesafioTargetSistemas.Domain.Repositories;

public interface ICustomerRepository
{
    Task<bool> ExistsByDocument(string document);
    Task Add(Customer customer);
}
