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
        
        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId);
    }
}