using Sales.Application.Strategy;

namespace Sales.Application.Interfaces;

public interface ICategoryFilterFactory
{
    ICategoryFilterStrategy GetStrategy(string filter);
}