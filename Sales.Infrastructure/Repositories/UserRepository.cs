using Microsoft.EntityFrameworkCore;
using Sales.Infrastructure.Context;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.Infrastructure.Repositories;

public class UserRepository(SalesDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<IEnumerable<User>> GetUsersOrders()
    {
        var employeesOrdersList = await _context.Users
            .AsNoTracking()
            .Include(e => e.Orders)
            .ToListAsync();

        return employeesOrdersList;
    }

    public async Task<User?> GetUserOrders(int id)
    {
        var employeeOrders = await _context.Users
            .AsNoTracking()
            .Include(e => e.Orders)
            .FirstOrDefaultAsync(e => e.UserId == id);
        
        return employeeOrders;
    }
}