using Sales.Application.DTOs.OrderDTO;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Domain.Models.Enums;

namespace Sales.Application.Strategy.FilterImplementation.OrderStrategy;

public class OrderStatusFilter : IOrderFilterStrategy
{
    public IEnumerable<OrderDTOOutput> ApplyFilter(IEnumerable<OrderDTOOutput> categories, OrderParameters parameters)
    {
        if (parameters.Status is not null)
        {
            categories = parameters.Status.ToLower() switch
            {
                "finished" => categories.Where(c => c.OrderStatus == Status.Finished),
                "preparing" => categories.Where(c => c.OrderStatus == Status.Preparing),
                _ => categories
            };
        }
        
        return categories.OrderBy(o => o.OrderDate);
    }

    public string GetFilter()
    {
        return "status";
    }
}