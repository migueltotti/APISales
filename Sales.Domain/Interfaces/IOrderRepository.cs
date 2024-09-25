using Sales.Domain.Models;

namespace Sales.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByProduct(string productName);  
    Task<IEnumerable<Product>> GetProductsByDate(DateTime minDate, DateTime maxDate);  
    Task<IEnumerable<Order>> GetOrdersByAffiliateId(int affiliateId);
    Task<int> AddProduct(int orderId, int productId);
    Task<IEnumerable<Product>> GetProducts(int orderId);
    Task<int> RemoveProduct(int orderId, int productId);
}