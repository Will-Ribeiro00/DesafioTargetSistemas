using DesafioTargetSistemas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DesafioTargetSistemas.Infrastructure.DataAccess
{
    public class DesafioTargetSistemasDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<AccountReceivable> AccountReceivables { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SalesItems { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DesafioTargetSistemasDbContext).Assembly);
        }
    }
}
