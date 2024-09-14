using Sales.Domain.Models;

namespace Sales.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<IEnumerable<User>> GetUsersOrders();
    Task<User?> GetUserOrders(int id);  
}