using DesafioTargetSistemas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Mapping
{
    public class ProductMapping : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("product");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(x => x.Code)
                .HasColumnName("code")
                .HasMaxLength(30)
                .IsUnicode(false)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Price)
                .HasColumnName("price")
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CurrentStock)
                .HasColumnName("current_stock");

            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}
