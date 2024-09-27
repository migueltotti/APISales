using Sales.Application.DTOs.OrderDTO;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Domain.Models.Enums;

namespace Sales.Application.Strategy.FilterImplementation.OrderStrategy;

public class OrderStatusFilter : IOrderFilterStrategy
{
    public IEnumerable<OrderDTOOutput> ApplyFilter(IEnumerable<OrderDTOOutput> orders, OrderParameters parameters)
    {
        if (parameters.Status is not null)
        {
            orders = parameters.Status.ToLower() switch
            {
                "finished" => orders.Where(c => c.OrderStatus == Status.Finished),
                "preparing" => orders.Where(c => c.OrderStatus == Status.Preparing),
                _ => orders
            };
        }
        
        return orders.OrderBy(o => o.OrderDate);
    }

    public string GetFilter()
    {
        return "status";
    }
}