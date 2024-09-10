using System.Linq.Expressions;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Domain.Models;

namespace Sales.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDTOOutput>> GetAllOrders();
    Task<OrderDTOOutput> GetOrderBy(Expression<Func<Order, bool>> expression);
    Task CreateOrder(OrderDTOInput order);
    Task UpdateOrder(OrderDTOInput order);
    Task DeleteOrder(int? id);
    Task AddProduct(ProductDTOInput product);
    Task RemoveProduct(ProductDTOInput product);
    Task<OrderReportDTO> GetOrderReport(DateTime startDate, DateTime endDate);    
}