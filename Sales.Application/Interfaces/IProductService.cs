using System.Linq.Expressions;
using Sales.Application.DTOs.ProductDTO;
using Sales.Domain.Models;

namespace Sales.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDTOOutput>> GetAllProducts();
    Task<ProductDTOOutput> GetProductBy(Expression<Func<Product, bool>> expression);
    Task CreateProduct(ProductDTOInput product);
    Task UpdateProduct(ProductDTOInput product);
    Task DeleteProduct(int? id);    
}