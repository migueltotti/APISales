using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Models;

namespace Sales.Infrastructure.ModelsConfiguration;

public class WorkDayConfiguration : IEntityTypeConfiguration<WorkDay>
{
    public void Configure(EntityTypeBuilder<WorkDay> builder)
    {
        builder.ToTable("WorkDay");
        builder.HasKey(w => w.WorkDayId);
        
        builder.Property(w => w.WorkDayId).ValueGeneratedOnAdd();
        builder.Property(w => w.EmployeeId)
            .IsRequired();
        builder.Property(w => w.EmployeeName)
            .IsRequired()
            .HasMaxLength(80);
        builder.Property(w => w.StartDayTime)
            .IsRequired();
        builder.Property(w => w.NumberOfOrders)
            .HasDefaultValue(0);
        builder.Property(w => w.NumberOfCanceledOrders)
            .HasDefaultValue(0);

        builder.HasOne(w => w.Employee)
            .WithMany(u => u.WorkDays)
            .HasForeignKey(w => w.EmployeeId);
    }
}