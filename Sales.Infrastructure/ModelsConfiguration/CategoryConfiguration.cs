using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Models;

namespace Sales.Infrastructure.ModelsConfiguration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");
        builder.HasKey(c => c.CategoryId);
        
        builder.Property(c => c.CategoryId).ValueGeneratedOnAdd();
        builder.Property(c => c.Name).HasMaxLength(50)
            .IsRequired();
        builder.Property(c => c.ImageUrl).HasMaxLength(250);
        
        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId);

        builder.HasData(
           new Category(1, "Carnes Bovinas", "carnes-bovinas.jpg"),
           new Category(2, "Produtos Diversos", "produtos-diversos.jpg"),
           new Category(3, "Aves", "aves.jpg"),
           new Category(4, "Carnes Suinas", "carnes-suinas.jpg")
        );
    }
}