namespace Sales.Domain.Models;

public sealed class Category
{
    public int CategoryId { get; private set; }
    public string? Name { get; private set; }
    public string? ImageUrl { get; private set; }
    
    // Category 1 : n Product
    public ICollection<Product>? Products { get; private set; }

    public Category(int categoryId, string? name, string? imageUrl)
    {
        CategoryId = categoryId;
        Name = name;
        ImageUrl = imageUrl;
    }

    public Category(string? name, string? imageUrl)
    {
        Name = name;
        ImageUrl = imageUrl;
    }
}