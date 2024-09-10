using System.Linq.Expressions;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Domain.Models;

namespace Sales.Application.Services;

public class OrderService : IOrderService
{
    public Task<IEnumerable<OrderDTOOutput>> GetAllOrders()
    {
        throw new NotImplementedException();
    }

    public Task<OrderDTOOutput> GetOrderBy(Expression<Func<Order, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task CreateOrder(OrderDTOInput order)
    {
        throw new NotImplementedException();
    }

    public Task UpdateOrder(OrderDTOInput order)
    {
        throw new NotImplementedException();
    }

    public Task DeleteOrder(int? id)
    {
        throw new NotImplementedException();
    }

    public Task AddProduct(ProductDTOInput product)
    {
        throw new NotImplementedException();
    }

    public Task RemoveProduct(ProductDTOInput product)
    {
        throw new NotImplementedException();
    }

    public Task<OrderReportDTO> GetOrderReport(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }
}