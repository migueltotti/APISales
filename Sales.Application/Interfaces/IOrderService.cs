using System.Linq.Expressions;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using Sales.Domain.Models.Enums;
using X.PagedList;

namespace Sales.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDTOOutput>> GetAllOrders();
    Task<IPagedList<OrderDTOOutput>> GetAllOrders(QueryStringParameters parameters);
    Task<Result<List<NumberOfProductDTO>>> Get5BestSellingProductsByNumberOfMonths(ProductParameters parameters);
    Task<IPagedList<OrderDTOOutput>> GetAllOrdersWithProductsByDateTimeNow(OrderParameters parameters);
    Task<Result<IPagedList<OrderWeekReportDTO>>> GetNumberOfOrdersFromTodayToLastSundays(OrderParameters parameters);
    Task<IPagedList<OrderDTOOutput>> GetOrdersWithFilter(string filter, OrderParameters parameters);
    Task<IPagedList<OrderDTOOutput>> GetOrdersByUserId(int userId, QueryStringParameters parameters);
    Task<IPagedList<OrderDTOOutput>> GetOrdersByProduct(OrderParameters parameters);
    Task<IPagedList<OrderDTOOutput>> GetOrdersByAffiliateId(int affiliateId, OrderParameters parameters);
    Task<Result<OrderDTOOutput>> GetOrderById(int id);
    Task<Result<OrderDTOOutput>> GetOrderBy(Expression<Func<Order, bool>> expression);
    Task<Result<OrderDTOOutput>> CreateOrder(OrderDTOInput order);
    Task<Result<OrderDTOOutput>> UpdateOrder(OrderDTOInput order, int id);
    Task<Result<OrderDTOOutput>> DeleteOrder(int? id);
    Task<Result<OrderDTOOutput>> CreateAndSendOrder(int userId, string? note = null);
    Task<Result<OrderDTOOutput>> SentOrder(int orderId, string? note = null);
    Task<Result<OrderDTOOutput>> FinishOrder(int orderId);
    Task<Result<OrderDTOOutput>> AddProduct(int orderId, int  productId, decimal amount);
    Task<Result<IEnumerable<ProductDTOOutput>>> GetProductsByOrderId(int orderId);
    Task<Result<OrderDTOOutput>> RemoveProduct(int orderId, int productId);
    Task<Result<OrderReportDTO>> GetOrderReport(DateTime? date);    
    Task<Result<object>> GenerateOrderReport(DateTime? date, ReportType reportType, string emailDest);   
}