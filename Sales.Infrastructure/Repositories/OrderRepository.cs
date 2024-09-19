using Microsoft.EntityFrameworkCore;
using Sales.Infrastructure.Context;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(SalesDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByProduct(string productName)
    {
        var orders = _context.Orders.FromSql(
            $"SELECT o.OrderId, o.TotalValue, o.OrderDate, o.UserId FROM salesdb.Order o JOIN salesdb.OrderProduct op On op.OrdersOrderId = o.OrderId JOIN salesdb.Product p ON op.ProductsProductId = p.ProductId WHERE p.Name LIKE (concat('%', {productName}, '%'))");
        
        return await orders.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByDate(DateTime minDate, DateTime maxDate)
    {
        var products = _context.Products.FromSql(
            $"SELECT p.ProductId, p.Name, p.Description, p.Value, p.TypeValue, p.ImageUrl, p.StockQuantity, p.CategoryId FROM salesdb.Order o JOIN salesdb.OrderProduct op On op.OrdersOrderId = o.OrderId JOIN salesdb.Product p ON op.ProductsProductId = p.ProductId WHERE o.OrderDate >= {minDate} AND  o.OrderDate <= {maxDate}");
        
        return await products.ToListAsync();
    }

    public async Task<int> AddProduct(int orderId, int productId)
    {
        var rowsAffected = await _context.Database.ExecuteSqlAsync($"INSERT INTO OrderProduct (OrdersOrderId, ProductsProductId) VALUES ({orderId}, {productId})");

        return rowsAffected;
    }

    public async Task<IEnumerable<Product>> GetProducts(int orderId)
    {
        var products = _context.Products.FromSql(
            $"SELECT p.ProductId, p.Name, p.Description, p.Value, p.TypeValue, p.ImageUrl, p.StockQuantity, p.CategoryId FROM OrderProduct op JOIN Product p ON op.ProductsProductId = p.ProductId Where OrdersOrderid = {orderId}");
        
        return await products.ToListAsync();
    }

    public Task<int> RemoveProduct(int orderId, int productId)
    {
        var rowsAffected = _context.Database.ExecuteSqlAsync(
            $"DELETE FROM OrderProduct WHERE OrdersOrderId = {orderId} AND ProductsProductId = {productId}");
        
        return rowsAffected;
    }
}