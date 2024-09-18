using System.Linq.Expressions;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList;

namespace Sales.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDTOOutput>> GetAllOrders();
    Task<IPagedList<OrderDTOOutput>> GetAllOrders(OrderParameters parameters);
    Task<Result<OrderDTOOutput>> GetOrderBy(Expression<Func<Order, bool>> expression);
    Task<Result<OrderDTOOutput>> CreateOrder(OrderDTOInput order);
    Task<Result<OrderDTOOutput>> UpdateOrder(OrderDTOInput order, int id);
    Task<Result<OrderDTOOutput>> DeleteOrder(int? id);
    Task<Result<OrderProductDTO>> AddProduct(int orderId, int  productId);
    Task<Result<IEnumerable<ProductDTOOutput>>> GetProductsByOrderId(int orderId);
    Task<Result<OrderProductDTO>> RemoveProduct(int orderId, int productId);
    Task<OrderReportDTO> GetOrderReport(DateTime startDate, DateTime endDate);    
}