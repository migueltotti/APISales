using Sales.Domain.Models;

namespace Sales.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByIdAsync(int id);  
    Task<User?> GetByEmailAsync(string email); 
    Task<IEnumerable<User>> GetUsersOrders();
    Task<User?> GetUserOrders(int id);  
}