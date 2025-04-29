using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Models;

namespace Sales.Infrastructure.ModelsConfiguration;

public class LineItemConfiguration : IEntityTypeConfiguration<LineItem>
{
    public void Configure(EntityTypeBuilder<LineItem> builder)
    {
        builder.ToTable("LineItem");
        builder.HasKey(li => li.LineItemId);
        
        builder.Property(li => li.LineItemId).ValueGeneratedOnAdd();
        builder.Property(li => li.OrderId).IsRequired();
        builder.Property(li => li.ProductId).IsRequired();
        builder.Property(li => li.Price).HasPrecision(8,2)
            .IsRequired();
        builder.Property(li => li.Amount).HasPrecision(8, 3)
            .IsRequired();
        
        builder.HasOne(li => li.Product)
            .WithMany()
            .HasForeignKey(li => li.ProductId);
        
        builder.HasOne(li => li.Order)
            .WithMany(o => o.LineItems)
            .HasForeignKey(li => li.OrderId);

        builder.HasData(
            new LineItem(1, 1, 1, 3, 3.5m),
            new LineItem(2, 1, 2, 1, 9.9m)
        );
    }
}