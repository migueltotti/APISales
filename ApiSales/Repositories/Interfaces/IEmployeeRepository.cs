using ApiSales.Models;

namespace ApiSales.Repositories.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> GetEmployeesOrders();
    Task<Employee?> GetEmployeeOrders(int id);
}