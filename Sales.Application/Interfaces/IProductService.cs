using System.Linq.Expressions;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.Parameters.ModelsParameters.ProductParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList;

namespace Sales.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDTOOutput>> GetAllProducts();
    Task<IPagedList<ProductDTOOutput>> GetAllProducts(QueryStringParameters parameters);
    Task<IPagedList<ProductDTOOutput>> GetProductsByValue(ProductFilterValue parameters);
    Task<IPagedList<ProductDTOOutput>> GetProductsByTypeValue(ProductFilterTypeValue parameters);
    Task<IPagedList<ProductDTOOutput>> GetProductsByName(ProductFilterName parameters);
    Task<Result<ProductDTOOutput>> GetProductBy(Expression<Func<Product, bool>> expression);
    Task<Result<ProductDTOOutput>> CreateProduct(ProductDTOInput product);
    Task<Result<ProductDTOOutput>> UpdateProduct(ProductDTOInput product, int id);
    Task<Result<ProductDTOOutput>> DeleteProduct(int? id);    
}