using System.Linq.Expressions;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Domain.Models;

namespace Sales.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDTOOutput>> GetAllCategories();    
    Task<CategoryDTOOutput> GetCategoryBy(Expression<Func<Category, bool>> expression);
    Task CreateCategory(CategoryDTOInput category);
    Task UpdateCategory(CategoryDTOInput category);
    Task DeleteCategory(int? id); 
}