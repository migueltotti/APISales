using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using X.PagedList;

namespace Sales.Application.Strategy.FilterImplementation.ProductStrategy;

public class ProductValueFilter : IProductFilterStrategy
{
    public IEnumerable<ProductDTOOutput> ApplyFilter(IEnumerable<ProductDTOOutput> products, ProductParameters parameters)
    {
        if (parameters is { Price: not null, PriceCriteria: not null })
        {
            products = parameters.PriceCriteria.ToLower() switch
            {
                "greater" => products.Where(p => p.Value > parameters.Price).OrderBy(p => p.Name),
                "equal" => products.Where(p => p.Value == parameters.Price).OrderBy(p => p.Name),
                "less" => products.Where(p => p.Value < parameters.Price).OrderBy(p => p.Name),
                _ => products
            };
        }
        
        return products;
    }

    public string GetFilter()
    {
        return "value";
    }
}