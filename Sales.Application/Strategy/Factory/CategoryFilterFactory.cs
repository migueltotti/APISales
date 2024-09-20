using Sales.Application.Interfaces;

namespace Sales.Application.Strategy.Factory;

public class CategoryFilterFactory : ICategoryFilterFactory
{
    private readonly Dictionary<string, ICategoryFilterStrategy> _strategies = new();

    public CategoryFilterFactory(IEnumerable<ICategoryFilterStrategy> strategies)
    {
        foreach (var filter in strategies)
        {
            _strategies.Add(filter.GetFilter(), filter);
        }
    }
    
    public ICategoryFilterStrategy GetStrategy(string filter)   
    {
        if (!_strategies.TryGetValue(filter.ToLower(), out ICategoryFilterStrategy strategy))       
        {
            throw new ArgumentException($"No strategy defined for {filter}");
        }
        
        return strategy;
    }
}