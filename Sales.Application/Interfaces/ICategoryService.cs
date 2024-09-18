using System.Linq.Expressions;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters.CategoryParameters;
using Sales.Application.Parameters.ModelsParameters.ProductParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList;

namespace Sales.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDTOOutput>> GetAllCategories();
    Task<IPagedList<CategoryDTOOutput>> GetAllCategories(QueryStringParameters parameters);
    Task<IPagedList<CategoryDTOOutput>> GetCategoriesByName(CategoryFilterName parameters);  
    Task<IPagedList<ProductDTOOutput>> GetProducts(int categoryId, QueryStringParameters parameters);
    Task<IPagedList<ProductDTOOutput>> GetProductsByValue(int categoryId, ProductFilterValue parameters);
    Task<Result<CategoryDTOOutput>> GetCategoryBy(Expression<Func<Category, bool>> expression);
    Task<Result<CategoryDTOOutput>> CreateCategory(CategoryDTOInput category);      
    Task<Result<CategoryDTOOutput>> UpdateCategory(CategoryDTOInput category, int id); 
    Task<Result<CategoryDTOOutput>> DeleteCategory(int? id); 
}