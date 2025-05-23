using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sales.Domain.Models;
using Sales.Domain.Models.Enums;

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
        builder.Property(o => o.OrderStatus).HasConversion<int>();
        builder.Property(o => o.Holder).HasMaxLength(50);
        builder.Property(o => o.Note).HasMaxLength(300);
    }
}