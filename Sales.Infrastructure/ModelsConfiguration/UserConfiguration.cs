using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Models;

namespace Sales.Infrastructure.ModelsConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(u => u.UserId);
        
        builder.Property(u => u.UserId).ValueGeneratedOnAdd();
        builder.Property(u => u.Name).HasMaxLength(80)
            .IsRequired();
        builder.Property(u => u.Email).HasMaxLength(80)
            .IsRequired();
        builder.Property(u => u.Password).HasMaxLength(30)
            .IsRequired();
        builder.Property(u => u.Cpf).HasMaxLength(14);
        builder.Property(u => u.DateBirth).HasColumnType("date");
        builder.Property(u => u.Role).HasConversion<int>()
            .IsRequired();

        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId);
    }
}