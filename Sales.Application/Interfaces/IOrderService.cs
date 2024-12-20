using System.Linq.Expressions;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList;

namespace Sales.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDTOOutput>> GetAllOrders();
    Task<IPagedList<OrderDTOOutput>> GetAllOrders(QueryStringParameters parameters);
    Task<IPagedList<OrderProductsDTO>> GetAllOrdersWithProductsByUserId(int userId, QueryStringParameters parameters);
    Task<IPagedList<OrderDTOOutput>> GetOrdersWithFilter(string filter, OrderParameters parameters);
    Task<IPagedList<OrderDTOOutput>> GetOrdersByUserId(int userId, QueryStringParameters parameters);
    Task<IPagedList<OrderDTOOutput>> GetOrdersByProduct(OrderParameters parameters);
    Task<IPagedList<OrderDTOOutput>> GetOrdersByAffiliateId(int affiliateId, OrderParameters parameters);
    Task<Result<OrderDTOOutput>> GetOrderById(int id);
    Task<Result<OrderDTOOutput>> GetOrderBy(Expression<Func<Order, bool>> expression);
    Task<Result<OrderDTOOutput>> CreateOrder(OrderDTOInput order);
    Task<Result<OrderDTOOutput>> UpdateOrder(OrderDTOInput order, int id);
    Task<Result<OrderDTOOutput>> DeleteOrder(int? id);
    Task<Result<OrderDTOOutput>> CreateAndSendOrder(int userId);
    Task<Result<OrderDTOOutput>> SentOrder(int orderId);
    Task<Result<OrderDTOOutput>> FinishOrder(int orderId);
    Task<Result<OrderProductsDTO>> AddProduct(int orderId, int  productId, decimal amount);
    Task<Result<IEnumerable<ProductDTOOutput>>> GetProductsByOrderId(int orderId);
    Task<Result<OrderProductsDTO>> RemoveProduct(int orderId, int productId);
    Task<OrderReportDTO> GetOrderReport(DateTime startDate, DateTime endDate);    
}