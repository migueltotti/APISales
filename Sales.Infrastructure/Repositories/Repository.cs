using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sales.Infrastructure.Context;
using Sales.Domain.Interfaces;

namespace Sales.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly SalesDbContext _context;
    
    public Repository(SalesDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var entitiesList = await _context.Set<T>().AsNoTracking().ToListAsync();

        return entitiesList;
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> expression)
    {
        var entity = await _context.Set<T>().FirstOrDefaultAsync(expression);

        return entity;
    }

    public T Create(T entity)
    {
        if (entity is null)
        {
            throw new ArgumentException(nameof(entity));
        }

        _context.Set<T>().Add(entity);

        return entity;
    }

    public T Update(T entity)
    {
        if (entity is null)
        {
            throw new ArgumentException(nameof(entity));
        }

        _context.Set<T>().Update(entity);

        return entity;
    }

    public T Delete(T entity)
    {
        if (entity is null)
        {
            throw new ArgumentException(nameof(entity));
        }

        _context.Set<T>().Remove(entity);

        return entity;
    }
}