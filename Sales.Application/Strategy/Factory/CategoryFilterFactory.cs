using Sales.Application.Interfaces;

namespace Sales.Application.Strategy.Factory;

public class CategoryFilterFactory : ICategoryFilterFactory
{
    private Dictionary<string, ICategoryFilterStrategy> _strategies { get; set; }

    public CategoryFilterFactory(List<ICategoryFilterStrategy> strategies)
    {
        foreach (var filter in strategies)
        {
            _strategies.Add(filter.GetFilter(), filter);
        }
    }
    
    public ICategoryFilterStrategy GetStrategy(string filter)   
    {
        throw new NotImplementedException();
    }
}