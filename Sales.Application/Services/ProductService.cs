using System.Linq.Expressions;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Domain.Models;

namespace Sales.Application.Services;

public class ProductService : IProductService
{
    public Task<IEnumerable<ProductDTOOutput>> GetAllProducts()
    {
        throw new NotImplementedException();
    }

    public Task<ProductDTOOutput> GetProductBy(Expression<Func<Product, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task CreateProduct(ProductDTOInput product)
    {
        throw new NotImplementedException();
    }

    public Task UpdateProduct(ProductDTOInput product)
    {
        throw new NotImplementedException();
    }

    public Task DeleteProduct(int? id)
    {
        throw new NotImplementedException();
    }
}