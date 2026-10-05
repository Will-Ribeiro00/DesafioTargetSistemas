using DesafioTargetSistemas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Mapping
{
    public class AccountReceivableMapping : IEntityTypeConfiguration<AccountReceivable>
    {
        public void Configure(EntityTypeBuilder<AccountReceivable> builder)
        {
            builder.ToTable("account_receivable");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(x => x.SaleId).HasColumnName("sale_id");

            builder.Property(x => x.Amount)
                .HasColumnName("amount")
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DueDate)
                .HasColumnName("due_date")
                .HasColumnType("date");

            builder.Property(x => x.PaymentDate)
                .HasColumnName("payment_date")
                .HasColumnType("date");

            builder.HasOne(x => x.Sale)
                .WithMany(s => s.AccountsReceivable)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
