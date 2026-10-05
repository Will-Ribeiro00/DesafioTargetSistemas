using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Mapping
{
    public class StockMovementsMapping : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.ToTable("stock_movement");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(x => x.ProductId).HasColumnName("product_id");
            builder.Property(x => x.SaleId).HasColumnName("sale_id");

            builder.Property(x => x.Type)
                .HasColumnName("type")
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasConversion(
                    v => v == StockMovementType.In ? "IN" : "OUT",
                    v => v == "IN" ? StockMovementType.In : StockMovementType.Out)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(200);

            builder.Property(x => x.Quantity).HasColumnName("quantity");
            builder.Property(x => x.StockBalance).HasColumnName("stock_balance");

            builder.Property(x => x.MovementDate)
                .HasColumnName("movement_date")
                .HasColumnType("datetime2(0)");

            builder.HasOne(x => x.Product)
                .WithMany(p => p.StockMovements)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Sale)
                .WithMany(s => s.StockMovements)
                .HasForeignKey(x => x.SaleId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
