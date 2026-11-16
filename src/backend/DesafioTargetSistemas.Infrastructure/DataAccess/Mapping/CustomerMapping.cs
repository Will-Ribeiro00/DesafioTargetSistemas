using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Mapping;

public class CustomerMapping : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customer");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.PersonType)
            .HasColumnName("person_type")
            .HasMaxLength(1)
            .IsFixedLength()
            .IsUnicode(false)
            .HasConversion(
                v => v == PersonType.Company ? "J" : "F",
                v => v == "J" ? PersonType.Company : PersonType.Individual)
            .IsRequired();

        builder.Property(x => x.Document)
            .HasColumnName("document")
            .HasMaxLength(14)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.TradeName)
            .HasColumnName("trade_name")
            .HasMaxLength(150);

        builder.Property(x => x.StateRegistration)
            .HasColumnName("state_registration")
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsUnicode(false);

        builder.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasMaxLength(11)
            .IsUnicode(false);

        builder.Property(x => x.SellerId).HasColumnName("seller_id");

        builder.Property(x => x.Active).HasColumnName("active");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime2(0)");

        builder.HasIndex(x => x.Document).IsUnique();

        builder.HasOne(x => x.Seller)
            .WithMany(s => s.Customers)
            .HasForeignKey(x => x.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
