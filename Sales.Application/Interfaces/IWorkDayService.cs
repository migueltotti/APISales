using Sales.Application.DTOs.WorkDayDTO;
using Sales.Application.ResultPattern;

namespace Sales.Application.Interfaces;

public interface IWorkDayService
{
    Task<Result<WorkDayDTOOutput>> GetWorkDayByIdAsync(int workDayId);
    Task<Result<WorkDayDTOOutput>> GetWorkDayByDateAsync(DateTime date);
    Task<Result<WorkDayDTOOutput>> StartWorkDay(WorkDayDTOInput workDay, int employeeId);
    Task<Result<WorkDayDTOOutput>> FinishWorkDay(int workDayId, int employeeId);
    Task<Result<WorkDayDTOOutput>> RegisterOrderToWorkDay();
    Task<Result<WorkDayDTOOutput>> CancelOrderToWorkDay();
}