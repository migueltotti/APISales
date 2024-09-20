using Sales.Application.Interfaces;

namespace Sales.Application.Strategy.Factory;

public class ProductFilterFactory : IProductFilterFactory
{
    private readonly Dictionary<string, IProductFilterStrategy> _strategies = new();

    public ProductFilterFactory(IEnumerable<IProductFilterStrategy> strategies)
    {
        foreach (var filter in strategies)
        {
            _strategies.Add(filter.GetFilter(), filter);
        }
    }
    
    public IProductFilterStrategy GetStrategy(string filter)   
    {
        if (!_strategies.TryGetValue(filter.ToLower(), out IProductFilterStrategy strategy))
        {
            throw new ArgumentException($"No strategy defined for {filter}");
        }
        
        return strategy;
    }
}