using Sales.Application.DTOs.OrderDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using X.PagedList;

namespace Sales.Application.Strategy.FilterImplementation.OrderStrategy;

public class OrderValueFilter : IOrderFilterStrategy
{
    public IEnumerable<OrderDTOOutput> ApplyFilter(IEnumerable<OrderDTOOutput> orders, OrderParameters parameters)
    {
        if (parameters is { Price: not null, PriceCriteria: not null })
        {
            orders = parameters.PriceCriteria.ToLower() switch
            {
                "greater" => orders.Where(o => o.TotalValue > parameters.Price)
                    .OrderBy(o => o.OrderDate),
                "equal" => orders.Where(o => o.TotalValue == parameters.Price)
                    .OrderBy(o => o.OrderDate),
                "less" => orders.Where(o => o.TotalValue < parameters.Price)
                    .OrderBy(o => o.OrderDate),
                _ => orders.OrderBy(o => o.OrderDate)
            };
        }
        
        return orders;
    }

    public string GetFilter()
    {
        return "value";
    }
}