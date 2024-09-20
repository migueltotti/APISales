using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.Parameters;
using X.PagedList;

namespace Sales.Application.Strategy;

public interface ICategoryFilterStrategy
{
    IEnumerable<CategoryDTOOutput> ApplyFilter(IEnumerable<CategoryDTOOutput> categories, CategoryParameters parameters);
    string GetFilter();
}