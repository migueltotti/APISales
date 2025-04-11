using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.WorkDayDTO;
using Sales.Domain.Models.Enums;

namespace Sales.Application.MassTransit.GenerateReport;

public record GeneratePOSReportEvent(
    Guid GenerateReportEventId,
    OrderReportDTO OrderReport,
    WorkDayDTOOutput WorkDayReport,
    ReportType ReportType
);