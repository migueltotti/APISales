using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Domain.Models.Enums;
using X.PagedList;

namespace Sales.Application.Strategy.FilterImplementation.ProductStrategy;

public class ProductTypeValueFilter : IProductFilterStrategy
{
    public IEnumerable<ProductDTOOutput> ApplyFilter(IEnumerable<ProductDTOOutput> products, ProductParameters parameters)
    {
        if (parameters is { TypeValue: not null })
        {
            products = parameters.TypeValue.ToLower() switch
            {
                "un" => products.Where(p => p.TypeValue == TypeValue.Uni).OrderBy(p => p.Name),
                "kg" => products.Where(p => p.TypeValue == TypeValue.Kg).OrderBy(p => p.Name),
                _ => products.OrderBy(p => p.Name)
            };
        }
        
        return products;
    }

    public string GetFilter()
    {
        return "typevalue";
    }
}