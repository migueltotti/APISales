using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualBasic;
using Sales.Infrastructure.Context;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(TestDbContext context) : base(context)
    {
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        return GetAsync(o => o.OrderId == id);
    }

    public async Task<IEnumerable<Order>> GetOrdersWithProductsByUserId(int userId)
    {
        var ordersProducts = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.LineItems)
            .ThenInclude(li => li.Product)
            .ToListAsync();
        
        return ordersProducts;
    }

    public async Task<IEnumerable<Order>> GetOrdersByProduct(string productName)
    {
        var orders = _context.Orders.FromSqlInterpolated(
            $""""
             SELECT o.* 
             FROM `order` o
             JOIN lineitem li ON li.OrderId = o.OrderId 
             JOIN product p ON p.ProductId = li.ProductId
             WHERE p.Name LIKE (concat('%', {productName}, '%'));
             """"
        );
        return await orders.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByDate(DateTime minDate, DateTime maxDate)
    {
        var products = _context.Products.FromSqlInterpolated(
            $"""
                 SELECT p.ProductId, p.Name, p.Description, p.Value, p.TypeValue, p.ImageUrl, p.StockQuantity, p.CategoryId
                 FROM `order` o
                 JOIN lineitem li On li.OrderId = o.OrderId
                 JOIN product p ON li.ProductId = p.ProductId
                 WHERE o.OrderDate >= {minDate} AND  o.OrderDate <= {maxDate}
              """);
        
        return await products.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByAffiliateId(int affiliateId)
    {
        var orders = _context.Orders.FromSqlInterpolated(
            $"""
               SELECT o.*
               FROM `order` o
               JOIN `user` u On u.UserId = o.UserId
               WHERE u.AffiliateId = {affiliateId}
            """);

        return await orders.ToListAsync();
    }

    public async Task<int> AddProduct(int orderId, int productId, decimal amount)   
    {
        var rowsAffected = await _context.Database.ExecuteSqlAsync($"INSERT INTO OrderProduct (OrdersOrderId, ProductsProductId, ProductAmount) VALUES ({orderId}, {productId}, {amount})");

        return rowsAffected;
    }
    
    public async Task<int> AddProductRange(int orderId, List<LineItem> products)
    {
        var addProductQuery = string.Join(", ", products.Select(p => 
            $"({orderId}, {p.Product.ProductId}, {p.Amount})"));
        
        var sqlQuery =
            $"INSERT INTO OrderProduct (OrdersOrderId, ProductsProductId, ProductAmount) VALUES {addProductQuery}";
        
        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sqlQuery);

        return rowsAffected;
    }

    public async Task<Order?> GetOrderWithProductsByOrderId(int orderId)
    {
        return await _context.Orders
            .Include(o => o.LineItems)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<IEnumerable<Product>> GetProducts(int orderId)
    {
        var products = _context.Products.FromSqlInterpolated(
            $"""
             SELECT p.* 
             FROM product p
             JOIN lineitem li ON li.ProductId = p.ProductId
             JOIN `order` o ON o.OrderId = li.OrderId
             WHERE o.OrderId = {orderId}
             """);
        
        return await products.ToListAsync();
    }
    
    public async Task<LineItem?> GetLineItemByOrderIdAndProductId(int orderId, int productId)
    {
        var lineItem = await _context.LineItems.FirstOrDefaultAsync(li => li.ProductId == productId && li.OrderId == orderId);
        return lineItem;
    }
    
    public async Task<IEnumerable<LineItem>?> GetLineItemsByOrderIdAndUserId(List<int> orderIds, int userId)
    {
        var lineItem = await _context.LineItems
            .Include(li => li.Product)
            .Where(li => li.Order.UserId == userId)
            .ToListAsync();
        
        return lineItem;
    }

    public Task<int> RemoveProduct(int orderId, int productId)
    {
        var rowsAffected = _context.Database.ExecuteSqlAsync(
            $"DELETE FROM OrderProduct WHERE OrdersOrderId = {orderId} AND ProductsProductId = {productId}");
        
        return rowsAffected;
    }
}