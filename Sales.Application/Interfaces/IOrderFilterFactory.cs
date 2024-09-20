using Sales.Application.Strategy;

namespace Sales.Application.Interfaces;

public interface IOrderFilterFactory
{
    IOrderFilterStrategy GetStrategy(string filter);
}