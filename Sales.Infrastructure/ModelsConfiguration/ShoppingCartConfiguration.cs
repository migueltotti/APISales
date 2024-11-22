using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Models;

namespace Sales.Infrastructure.ModelsConfiguration;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.ToTable("ShoppingCart");
        builder.HasKey(sh => sh.ShoppingCartId);
        
        builder.Property(sh => sh.ShoppingCartId).ValueGeneratedOnAdd();
        builder.Property(sh => sh.UserId)
            .IsRequired();
        builder.HasIndex(sh => sh.UserId)
            .IsUnique();
        builder.Property(sh => sh.TotalValue).HasPrecision(10, 2);
        builder.Property(sh => sh.ProductsCount).HasDefaultValue(0);

        builder.HasMany(sh => sh.Products)
            .WithMany(p => p.ShoppingCart)
            .UsingEntity<ShoppingCartProduct>(j =>
            {
                j.Property(shc => shc.Checked).HasDefaultValue(true);
                j.Property(shc => shc.Amount).HasPrecision(10, 3);
            });
        
        builder.HasOne(sh => sh.User);
    }
}