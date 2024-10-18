using Sales.Domain.Models;

namespace Sales.Domain.Interfaces;

public interface IAffiliateRepository : IRepository<Affiliate>
{
    Task<Affiliate?> GetByIdAsync(int id);
}