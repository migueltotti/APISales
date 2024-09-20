using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        builder.Property(u => u.Password).HasMaxLength(30)
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
            new User(1, "Miguel Totti de Oliveira", "migueltotti2005@gmail.com", "testemiguel","111.111.111-11", 0.00m, new DateTime(0001, 01, 01), Role.Admin, 1),
            new User(2, "Isadora Leao Paludeto", "isadorapaludeto15@gmail.com", "testeisadora","222.222.222-22", 0.00m, new DateTime(0002, 02, 02), Role.Admin, 1),
            new User(31, "TesteAdmin", "testeadmin@gmail.com", "testeadmin","331.331.331-31", 0.00m, new DateTime(0003, 03, 03), Role.Admin, 1),
            new User(32, "TesteEmployee", "testeemployee@gmail.com", "testeemployee","332.332.332-32", 0.00m, new DateTime(0003, 03, 03), Role.Employee, 1),
            new User(33, "TesteCustomer", "testecustomer@gmail.com", "testecustomer","333.333.333-33", 0.00m, new DateTime(0003, 03, 03), Role.Customer, 1)
        );
    }
}