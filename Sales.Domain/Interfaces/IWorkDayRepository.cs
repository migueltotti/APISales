using Sales.Domain.Models;

namespace Sales.Domain.Interfaces;

public interface IWorkDayRepository : IRepository<WorkDay>
{
    Task<WorkDay?> GetByIdAsync(int id);
}