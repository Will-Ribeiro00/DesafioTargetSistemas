using DesafioTargetSistemas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Mapping
{
    public class SaleItemMapping : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.ToTable("sale_item");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(x => x.SaleId).HasColumnName("sale_id");
            builder.Property(x => x.ProductId).HasColumnName("product_id");
            builder.Property(x => x.Quantity).HasColumnName("quantity");

            builder.Property(x => x.UnitPrice)
                .HasColumnName("unit_price")
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Sale)
                .WithMany(s => s.Items)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
                .WithMany(p => p.SaleItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
