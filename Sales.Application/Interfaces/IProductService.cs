using System.Linq.Expressions;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList;

namespace Sales.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDTOOutput>> GetAllProducts();
    Task<IPagedList<ProductDTOOutput>> GetAllProducts(ProductParameters parameters);
    Task<Result<ProductDTOOutput>> GetProductBy(Expression<Func<Product, bool>> expression);
    Task<Result<ProductDTOOutput>> CreateProduct(ProductDTOInput product);
    Task<Result<ProductDTOOutput>> UpdateProduct(ProductDTOInput product, int id);
    Task<Result<ProductDTOOutput>> DeleteProduct(int? id);    
}