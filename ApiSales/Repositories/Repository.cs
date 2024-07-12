using System.Linq.Expressions;
using ApiSales.Context;
using ApiSales.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ApiSales.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApiSalesDbContext _context;
    
    public Repository(ApiSalesDbContext context)
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