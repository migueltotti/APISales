using System.Collections;
using Sales.Domain.Models;

namespace Sales.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdAsync(int id); 
    Task<IEnumerable<Order>> GetOrdersByProduct(string productName);  
    Task<IEnumerable<Product>> GetProductsByDate(DateTime minDate, DateTime maxDate);  
    Task<IEnumerable<Order>> GetOrdersByAffiliateId(int affiliateId);
    Task<Order> GetOrderProductsById(int orderId);
    Task<int> AddProduct(int orderId, int productId, decimal amount);
    Task<IEnumerable<Product>> GetProducts(int orderId);
    Task<IEnumerable<ProductInfo>> GetProductValueAndAmount(int orderId, int productId);
    Task<int> RemoveProduct(int orderId, int productId);
}