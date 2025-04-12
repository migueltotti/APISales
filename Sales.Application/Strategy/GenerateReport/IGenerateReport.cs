using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.WorkDayDTO;
using Sales.Domain.Models.Enums;

namespace Sales.Application.Strategy.GenerateReport;

public interface IGenerateReport
{
    Task<MemoryStream> GenerateReportAsync(OrderReportDTO orderReport, WorkDayDTOOutput workDayReport, CancellationToken cancellationToken = default);
    ReportType GetReportType();
}