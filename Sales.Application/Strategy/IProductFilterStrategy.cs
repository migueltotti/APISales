using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using X.PagedList;

namespace Sales.Application.Strategy;

public interface IProductFilterStrategy
{
    IEnumerable<ProductDTOOutput> ApplyFilter(IEnumerable<ProductDTOOutput> categories, ProductParameters parameters);
    string GetFilter();
}