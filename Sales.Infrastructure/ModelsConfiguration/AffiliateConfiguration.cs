using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Models;

namespace Sales.Infrastructure.ModelsConfiguration;

public class AffiliateConfiguration : IEntityTypeConfiguration<Affiliate>
{
    public void Configure(EntityTypeBuilder<Affiliate> builder)
    {
        builder.ToTable("Affiliate");

        builder.HasKey(a => a.AffiliateId);
        
        builder.Property(a => a.AffiliateId).ValueGeneratedOnAdd();
        builder.Property(a => a.Name).HasMaxLength(50)
            .IsRequired();
        builder.Property(a => a.Discount).HasPrecision(4,2)
            .IsRequired();
        
        builder.HasMany(a => a.Users)
            .WithOne(u => u.Affiliate)
            .HasForeignKey(u => u.AffiliateId);

        builder.HasData(
            new Affiliate(1, "Nenhuma Afiliação", 0.00m)
        );
    }
}