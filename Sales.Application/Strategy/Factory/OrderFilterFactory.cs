using Sales.Application.Interfaces;

namespace Sales.Application.Strategy.Factory;

public class OrderFilterFactory : IOrderFilterFactory
{
    private readonly Dictionary<string, IOrderFilterStrategy> _strategies = new();

    public OrderFilterFactory(IEnumerable<IOrderFilterStrategy> strategies)
    {
        foreach (var filter in strategies)
        {
            _strategies.Add(filter.GetFilter(), filter);
        }
    }
    
    public IOrderFilterStrategy GetStrategy(string filter)   
    {
        if (!_strategies.TryGetValue(filter.ToLower(), out IOrderFilterStrategy strategy))       
        {
            throw new ArgumentException($"No strategy defined for {filter}");
        }
        
        return strategy;
    }
}