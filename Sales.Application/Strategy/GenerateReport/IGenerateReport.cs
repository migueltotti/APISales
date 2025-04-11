using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.WorkDayDTO;
using Sales.Domain.Models.Enums;

namespace Sales.Application.Interfaces;

public interface IGenerateReport
{
    Task GenerateReportAsync(OrderReportDTO orderReport, WorkDayDTOOutput workDayReport, CancellationToken cancellationToken = default);
    ReportType GetReportType();
}