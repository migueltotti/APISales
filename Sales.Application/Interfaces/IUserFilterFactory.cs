using Sales.Application.Strategy;

namespace Sales.Application.Interfaces;

public interface IUserFilterFactory
{
    IUserFilterStrategy GetStrategy(string filter);
}