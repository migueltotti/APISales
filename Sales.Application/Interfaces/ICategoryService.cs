using System.Linq.Expressions;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;

namespace Sales.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDTOOutput>> GetAllCategories();    
    Task<Result<CategoryDTOOutput>> GetCategoryBy(Expression<Func<Category, bool>> expression);
    Task<Result<CategoryDTOOutput>> CreateCategory(CategoryDTOInput category);      
    Task<Result<CategoryDTOOutput>> UpdateCategory(CategoryDTOInput category, int id); 
    Task<Result<CategoryDTOOutput>> DeleteCategory(int? id); 
}