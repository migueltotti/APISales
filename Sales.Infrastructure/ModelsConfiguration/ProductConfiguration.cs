using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Sales.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sales.Infrastructure.ModelsConfiguration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");
        builder.HasKey(x => x.ProductId);
        // REFACTORY: Change Id to GUID, implement GUID.Generator and save on DB
        builder.Property(x => x.ProductId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.Description).HasMaxLength(175);
        builder.Property(x => x.Value).HasPrecision(10, 2)
            .IsRequired();
        builder.Property(x => x.TypeValue).HasConversion<int>()
            .IsRequired();
        builder.Property(x => x.ImageUrl).HasMaxLength(250);
        builder.Property(x => x.StockQuantity).HasDefaultValue(0)
            .IsRequired();

        builder.HasMany(x => x.Orders)
            .WithMany(x => x.Products);
    }
}