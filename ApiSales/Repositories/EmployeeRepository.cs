using ApiSales.Context;
using ApiSales.Models;
using ApiSales.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiSales.Repositories;

public class EmployeeRepository(ApiSalesDbContext context) : Repository<Employee>(context), IEmployeeRepository
{
    public async Task<IEnumerable<Employee>> GetEmployeesOrders()
    {
        var employeesOrdersList = await _context.Employees
            .AsNoTracking()
            .Include(e => e.Orders)
            .ToListAsync();

        return employeesOrdersList;
    }

    public async Task<Employee?> GetEmployeeOrders(int id)
    {
        var employeeOrders = await _context.Employees
            .AsNoTracking()
            .Include(e => e.Orders)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);
        
        return employeeOrders;
    }
}