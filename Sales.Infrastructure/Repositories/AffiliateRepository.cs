using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Context;

namespace Sales.Infrastructure.Repositories;

public class AffiliateRepository(SalesDbContext context) : Repository<Affiliate>(context), IAffiliateRepository;