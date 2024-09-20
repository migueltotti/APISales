using Sales.Application.Interfaces;

namespace Sales.Application.Strategy.Factory;

public class ProductFilterFactory : IProductFilterFactory
{
    private Dictionary<string, IProductFilterStrategy> _strategies { get; set; }

    public ProductFilterFactory(List<IProductFilterStrategy> strategies)
    {
        foreach (var filter in strategies)
        {
            _strategies.Add(filter.GetFilter(), filter);
        }
    }
    
    public IProductFilterStrategy GetStrategy(string filter)   
    {
        throw new NotImplementedException();
    }
}