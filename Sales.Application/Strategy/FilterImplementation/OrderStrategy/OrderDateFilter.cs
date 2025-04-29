using Sales.Application.DTOs.OrderDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using X.PagedList;

namespace Sales.Application.Strategy.FilterImplementation.OrderStrategy;

public class OrderDateFilter : IOrderFilterStrategy
{
    public IEnumerable<OrderDTOOutput> ApplyFilter(IEnumerable<OrderDTOOutput> orders, OrderParameters parameters)
    {
        if (parameters.From is not null)
        {
            orders = parameters.To switch
            {
                null => orders.Where(o => o.OrderDate.Day >= parameters.From.Value.Day)
                    .OrderBy(o => o.OrderDate),
                not null => orders.Where(o => o.OrderDate.Day  >= parameters.From.Value.Day &&
                                                            o.OrderDate.Day  <= parameters.To.Value.Day)
                    .OrderBy(o => o.OrderDate)
            };
        }

        return orders;
    }

    public string GetFilter()
    {
        return "date";
    }
}