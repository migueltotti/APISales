using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Sales.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Models.Enums;

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

        builder.HasData(
            new Product(1, "Coca-Cola 250", "Coca Cola 250ml garrafinha", 3.5m, TypeValue.Uni, "coca-cola-250.jpg", 10, 2),
            new Product(2, "Pão Caseiro", "Pão Caseiro feito no dia", 9.9m, TypeValue.Uni, "pao-caseiro.jpg", 3, 2),
            new Product(3, "Picanha", "Picanha", 69.99m, TypeValue.Kg, "picanha.jpg", 5, 1)
        );
    }
}