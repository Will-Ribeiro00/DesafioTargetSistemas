using DesafioTargetSistemas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Mapping;

public class CustomerAddressMapping : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("customer_address");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.CustomerId).HasColumnName("customer_id");

        builder.Property(x => x.ZipCode)
            .HasColumnName("zip_code")
            .HasMaxLength(8)
            .IsFixedLength()
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.Street)
            .HasColumnName("street")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Number)
            .HasColumnName("number")
            .HasMaxLength(10)
            .IsUnicode(false);

        builder.Property(x => x.Complement)
            .HasColumnName("complement")
            .HasMaxLength(60);

        builder.Property(x => x.District)
            .HasColumnName("district")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(x => x.City)
            .HasColumnName("city")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(x => x.State)
            .HasColumnName("state")
            .HasMaxLength(2)
            .IsFixedLength()
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.IsDefault).HasColumnName("is_default");

        builder.HasIndex(x => x.CustomerId)
            .IsUnique()
            .HasFilter("[is_default] = 1");

        builder.HasOne(x => x.Customer)
            .WithMany(c => c.Addresses)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
