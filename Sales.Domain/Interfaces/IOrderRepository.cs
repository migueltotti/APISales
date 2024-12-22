using System.Collections;
using Sales.Domain.Models;

namespace Sales.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdAsync(int id); 
    Task<IEnumerable<Order>> GetOrdersByProduct(string productName);  
    Task<IEnumerable<Order>> GetOrdersWithProductsByUserId(int userId); 
    Task<IEnumerable<Product>> GetProductsByDate(DateTime minDate, DateTime maxDate);  
    Task<IEnumerable<Order>> GetOrdersByAffiliateId(int affiliateId);
    Task<Order?> GetOrderWithProductsByOrderId(int orderId);
    Task<int> AddProduct(int orderId, int productId, decimal amount);
    Task<IEnumerable<Product>> GetProducts(int orderId);
    Task<LineItem?> GetLineItemByOrderIdAndProductId(int orderId, int productId);
    Task<IEnumerable<LineItem>?> GetLineItemsByOrderIdAndUserId(List<int> orderIds, int userId);
    Task<int> RemoveProduct(int orderId, int productId);
}