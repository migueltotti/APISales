using Sales.Application.Interfaces;

namespace Sales.Application.Strategy.Factory;

public class UserFilterFactory : IUserFilterFactory
{
    private Dictionary<string, IUserFilterStrategy> _strategies { get; set; }

    public UserFilterFactory(List<IUserFilterStrategy> strategies)
    {
        foreach (var filter in strategies)
        {
            _strategies.Add(filter.GetFilter(), filter);
        }
    }
    
    public IUserFilterStrategy GetStrategy(string filter)   
    {
        throw new NotImplementedException();
    }
}