using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Context;

namespace Sales.Infrastructure.Repositories;

public class AffiliateRepository(MergeDbContext context) : Repository<Affiliate>(context), IAffiliateRepository
{
    public Task<Affiliate?> GetByIdAsync(int id)
    {
        return GetAsync(a => a.AffiliateId == id);
    }
}