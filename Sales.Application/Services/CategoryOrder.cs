using System.Linq.Expressions;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.Interfaces;
using Sales.Domain.Models;

namespace Sales.Application.Services;

public class CategoryOrder : ICategoryService
{
    public Task<IEnumerable<CategoryDTOOutput>> GetAllCategories()
    {
        throw new NotImplementedException();
    }

    public Task<CategoryDTOOutput> GetCategoryBy(Expression<Func<Category, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task CreateCategory(CategoryDTOInput category)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCategory(CategoryDTOInput category)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCategory(int? id)
    {
        throw new NotImplementedException();
    }
}