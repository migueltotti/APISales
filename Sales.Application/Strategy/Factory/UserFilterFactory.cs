using Sales.Application.Interfaces;

namespace Sales.Application.Strategy.Factory;

public class UserFilterFactory : IUserFilterFactory
{
    private readonly Dictionary<string, IUserFilterStrategy> _strategies = new();   

    public UserFilterFactory(IEnumerable<IUserFilterStrategy> strategies)
    {
        foreach (var filter in strategies)
        {
            _strategies.Add(filter.GetFilter(), filter);
        }
    }
    
    public IUserFilterStrategy GetStrategy(string filter)   
    {
        if (!_strategies.TryGetValue(filter.ToLower(), out IUserFilterStrategy strategy))        
        {
            throw new ArgumentException($"No strategy defined for {filter}");
        }
        
        return strategy;
    }
}