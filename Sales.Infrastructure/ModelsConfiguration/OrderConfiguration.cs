using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Models;

namespace Sales.Infrastructure.ModelsConfiguration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder) 
    {
        builder.ToTable("Order");
        builder.HasKey(o => o.OrderId);
        
        builder.Property(o => o.OrderId).ValueGeneratedOnAdd();
        builder.Property(o => o.TotalValue).HasPrecision(10,2)
            .IsRequired();
        builder.Property(o => o.OrderDate).HasColumnType("datetime")
            .IsRequired();
    }
}