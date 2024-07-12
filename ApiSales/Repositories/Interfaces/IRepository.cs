using System.Linq.Expressions;

namespace ApiSales.Repositories.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetAsync(Expression<Func<T, bool>> expression);
    T Create(T entity);
    T Update(T entity);
    T Delete(T entity);
}