using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Domain.Models.Enums;

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
        builder.Property(u => u.Password).HasMaxLength(400)
            .IsRequired();
        builder.Property(u => u.Cpf).HasMaxLength(14);
        builder.Property(u => u.Points).HasPrecision(6, 2);
        builder.Property(u => u.DateBirth).HasColumnType("date");
        builder.Property(u => u.Role).HasConversion<int>()
            .IsRequired();

        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId);

        builder.HasData(
            new User(
                1, 
                "Admin", 
                "admin@gmail.com", 
                "dHD+oA/Wkqs3YJ4JdWblRNFixjj8A2b2R4d+K2GNfKGhr7i56EwQ2YgFYcdbTAXFwnYEyjFjloYhCYcdiBJZOZpy/Q99ZDmk/fHTGOl3oTgQluSsV00wDwth1xaqVOsiuzG9YyNKeL1VdFTT1BW++Y3k3SxhC/niNC4od384zEU=",
                "000.000.000-00", 
                0.00m, 
                new DateTime(0001, 01, 01), 
                Role.Admin, 
                1
            )
        );
    }
}