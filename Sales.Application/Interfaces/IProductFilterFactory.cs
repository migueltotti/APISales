using Sales.Application.Strategy;

namespace Sales.Application.Interfaces;

public interface IProductFilterFactory
{
    IProductFilterStrategy GetStrategy(string filter);
}