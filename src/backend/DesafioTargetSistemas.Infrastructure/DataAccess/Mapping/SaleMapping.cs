using DesafioTargetSistemas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Mapping
{
    public class SaleMapping : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("sale");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(x => x.SellerId).HasColumnName("seller_id");

            builder.Property(x => x.SaleDate)
                .HasColumnName("sale_date")
                .HasColumnType("datetime2(0)");

            builder.Property(x => x.TotalPrice)
                .HasColumnName("total_price")
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CommissionPercentage)
                .HasColumnName("commission_percentage")
                .HasColumnType("decimal(5,2)");

            builder.Property(x => x.CommissionAmount)
                .HasColumnName("commission_amount")
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Seller)
                .WithMany(s => s.Sales)
                .HasForeignKey(x => x.SellerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
