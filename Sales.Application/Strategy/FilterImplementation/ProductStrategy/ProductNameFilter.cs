using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using X.PagedList;

namespace Sales.Application.Strategy.FilterImplementation.ProductStrategy;

public class ProductNameFilter : IProductFilterStrategy
{
    public IEnumerable<ProductDTOOutput> ApplyFilter(IEnumerable<ProductDTOOutput> products, ProductParameters parameters)
    {
        if (parameters.Name is not null)
        {
            products = products.Where(p => p.Name.Contains(parameters.Name,
                    StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(p => p.Name);
        }

        return products;
    }

    public string GetFilter()
    {
        return "name";
    }
}