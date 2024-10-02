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

        builder.HasData(
            new Order(1, 10.00m, new DateTime(2024, 09, 19, 15, 50, 45), 1, Status.Finished),
            new Order(2, 20.00m, new DateTime(2024, 09, 20, 15, 50, 45), 2, Status.Finished),
            new Order(3, 30.00m, new DateTime(2024, 09, 19, 15, 51, 39), 1),
            new Order(4, 40.00m, new DateTime(2024, 09, 19, 15, 53, 36), 2, Status.Finished),
            new Order(5, 0.00m, new DateTime(2024, 09, 20, 17, 47, 58), 2),
            new Order(6, 83.49m, new DateTime(2024, 09, 30, 8, 33, 16), 2, Status.Finished)
            );
    }
}