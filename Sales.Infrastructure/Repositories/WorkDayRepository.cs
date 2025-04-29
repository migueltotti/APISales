using Microsoft.EntityFrameworkCore;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Infrastructure.Context;

namespace Sales.Infrastructure.Repositories;

public class WorkDayRepository : Repository<WorkDay>, IWorkDayRepository
{
    public WorkDayRepository(TestDbContext context) : base(context)
    {
    }

    public async Task<WorkDay?> GetByIdAsync(int id)
    {
        return await _context.WorkDays
            .AsNoTracking()
            .Where(wd => wd.WorkDayId.Equals(id))
            .Include(wd => wd.Employee)
            .FirstOrDefaultAsync();
    }
}