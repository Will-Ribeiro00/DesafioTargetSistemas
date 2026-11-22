using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Repositories;

public class CustomerRepository(DesafioTargetSistemasDbContext dbContext) : ICustomerRepository
{
    private readonly DesafioTargetSistemasDbContext _dbContext = dbContext;

    public async Task<bool> ExistsByDocument(string document) => await _dbContext.Customers.AnyAsync(c => c.Document == document);

    public async Task Add(Customer customer) => await _dbContext.Customers.AddAsync(customer);
}
