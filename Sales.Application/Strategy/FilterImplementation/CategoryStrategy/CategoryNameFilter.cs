using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.Parameters;
using X.PagedList;

namespace Sales.Application.Strategy.FilterImplementation.CategoryStrategy;

public class CategoryNameFilter : ICategoryFilterStrategy
{
    public IEnumerable<CategoryDTOOutput> ApplyFilter(IEnumerable<CategoryDTOOutput> categories, CategoryParameters parameters)  
    {
        if (parameters.Name is not null)
        {
            categories = categories.Where(c => c.Name.Contains(parameters.Name,
                    StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(c => c.Name);
        } 
        
        return categories;
    }

    public string GetFilter()
    {
        return "name";
    }
}