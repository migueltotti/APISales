using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualBasic;
using Sales.Infrastructure.Context;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Domain.Models.Enums;

namespace Sales.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(SalesDbContext context) : base(context)
    {
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        return GetAsync(o => o.OrderId == id);
    }

    public async Task<IEnumerable<Order>> GetAllOrdersWithProductsByLastMonths(int monthCount)
    {
        var ordersProductsByLastMonths = await _context.Orders
            .Where(o => o.OrderDate >= DateTime.Now.AddMonths(-monthCount))
            .Include(o => o.LineItems)
            .ThenInclude(li => li.Product)
            .ToListAsync();
        
        return ordersProductsByLastMonths;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersWithProductsByDate(DateTime date)
    {
        var formatedDate = DateOnly.FromDateTime(date);
        
        var ordersProductsByDateNow = await _context.Orders
            .Where(o => DateOnly.FromDateTime(o.OrderDate).Equals(formatedDate))
            .Include(o => o.LineItems)
            .ThenInclude(li => li.Product)
            .ToListAsync();
        
        return ordersProductsByDateNow;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersWithProductsByTodayDate(Status orderStatus)
    {
        var ordersProductsByDateNow = await _context.Orders
            .Where(o => o.OrderDate.Day == DateTime.Now.Day && o.OrderStatus == orderStatus)
            .Include(o => o.LineItems)
            .ThenInclude(li => li.Product)
            .ToListAsync();
        
        return ordersProductsByDateNow;
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
             FROM `Order` o
             JOIN LineItem li ON li.OrderId = o.OrderId 
             JOIN Product p ON p.ProductId = li.ProductId
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
                 FROM `Order` o
                 JOIN LineItem li On li.OrderId = o.OrderId
                 JOIN Product p ON li.ProductId = p.ProductId
                 WHERE o.OrderDate >= {minDate} AND  o.OrderDate <= {maxDate}
              """);
        
        return await products.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByAffiliateId(int affiliateId)
    {
        var orders = _context.Orders.FromSqlInterpolated(
            $"""
               SELECT o.*
               FROM `Order` o
               JOIN `User` u On u.UserId = o.UserId
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
            .AsNoTracking()
            .Include(o => o.LineItems)
            .ThenInclude(li => li.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<IEnumerable<Product>> GetProducts(int orderId)
    {
        var products = _context.Products.FromSqlInterpolated(
            $"""
             SELECT p.* 
             FROM Product p
             JOIN LineItem li ON li.ProductId = p.ProductId
             JOIN `Order` o ON o.OrderId = li.OrderId
             WHERE o.OrderId = {orderId}
             """).AsNoTracking();
        
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