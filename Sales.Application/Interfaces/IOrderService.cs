using System.Linq.Expressions;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;

namespace Sales.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDTOOutput>> GetAllOrders();
    Task<Result<OrderDTOOutput>> GetOrderBy(Expression<Func<Order, bool>> expression);
    Task<Result<OrderDTOOutput>> CreateOrder(OrderDTOInput order);
    Task<Result<OrderDTOOutput>> UpdateOrder(OrderDTOInput order, int id);
    Task<Result<OrderDTOOutput>> DeleteOrder(int? id);
    Task AddProduct(ProductDTOInput product);
    Task RemoveProduct(ProductDTOInput product);
    Task<OrderReportDTO> GetOrderReport(DateTime startDate, DateTime endDate);    
}