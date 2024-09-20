using Sales.Application.Interfaces;

namespace Sales.Application.Strategy.Factory;

public class OrderFilterFactory : IOrderFilterFactory
{
    private Dictionary<string, IOrderFilterStrategy> _strategies { get; set; }

    public OrderFilterFactory(List<IOrderFilterStrategy> strategies)
    {
        foreach (var filter in strategies)
        {
            _strategies.Add(filter.GetFilter(), filter);
        }
    }
    
    public IOrderFilterStrategy GetStrategy(string filter)   
    {
        throw new NotImplementedException();
    }
}