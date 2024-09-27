using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using X.PagedList;

namespace Sales.Application.Strategy;

public interface IOrderFilterStrategy
{
    IEnumerable<OrderDTOOutput> ApplyFilter(IEnumerable<OrderDTOOutput> orders, OrderParameters parameters);
    string GetFilter();
}