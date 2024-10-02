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
        var orders = _context.Orders.FromSqlInterpolated(
            $"SELECT o.OrderId, o.TotalValue, o.OrderDate, o.UserId, o.OrderStatus FROM salesdb.Order o JOIN salesdb.OrderProduct op On op.OrdersOrderId = o.OrderId JOIN salesdb.Product p ON op.ProductsProductId = p.ProductId WHERE p.Name LIKE (concat('%', {productName}, '%'))");
        
        return await orders.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByDate(DateTime minDate, DateTime maxDate)
    {
        var products = _context.Products.FromSqlInterpolated(
            $"SELECT p.ProductId, p.Name, p.Description, p.Value, p.TypeValue, p.ImageUrl, p.StockQuantity, p.CategoryId FROM salesdb.Order o JOIN salesdb.OrderProduct op On op.OrdersOrderId = o.OrderId JOIN salesdb.Product p ON op.ProductsProductId = p.ProductId WHERE o.OrderDate >= {minDate} AND  o.OrderDate <= {maxDate}");
        
        return await products.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByAffiliateId(int affiliateId)
    {
        var orders = _context.Orders.FromSqlInterpolated(
            $"SELECT o.OrderId, o.TotalValue, o.OrderDate, o.UserId, o.OrderStatus FROM salesdb.Order o JOIN salesdb.user u ON u.UserId = o.UserId where u.AffiliateId = {affiliateId}");

        return await orders.ToListAsync();
    }

    public async Task<int> AddProduct(int orderId, int productId, decimal amount)   
    {
        var rowsAffected = await _context.Database.ExecuteSqlAsync($"INSERT INTO OrderProduct (OrdersOrderId, ProductsProductId, ProductAmount) VALUES ({orderId}, {productId}, {amount})");

        return rowsAffected;
    }

    public async Task<Order> GetOrderProductsById(int orderId)
    {
        return await _context.Orders
            .Include(o => o.Products)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<IEnumerable<Product>> GetProducts(int orderId)
    {
        var products = _context.Products.FromSqlInterpolated(
            $"SELECT p.ProductId, p.Name, p.Description, p.Value, p.TypeValue, p.ImageUrl, p.StockQuantity, p.CategoryId FROM OrderProduct op JOIN Product p ON op.ProductsProductId = p.ProductId Where OrdersOrderid = {orderId}");
        
        return await products.ToListAsync();
    }
    
    public async Task<IEnumerable<ProductInfo>> GetProductValueAndAmount(int orderId, int productId)
    {
        var productAmount = _context.Database.SqlQuery<ProductInfo>(
                $"SELECT p.value, op.ProductAmount FROM salesdb.orderproduct op JOIN salesdb.product p ON p.ProductId = op.ProductsProductId WHERE OrdersOrderId = {orderId} AND ProductsProductId = {productId};");
        
        return await productAmount.ToListAsync();
    }

    public Task<int> RemoveProduct(int orderId, int productId)
    {
        var rowsAffected = _context.Database.ExecuteSqlAsync(
            $"DELETE FROM OrderProduct WHERE OrdersOrderId = {orderId} AND ProductsProductId = {productId}");
        
        return rowsAffected;
    }
}