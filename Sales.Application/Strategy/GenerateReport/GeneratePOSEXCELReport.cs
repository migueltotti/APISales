using Microsoft.Extensions.Logging;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.WorkDayDTO;
using Sales.Domain.Models.Enums;

namespace Sales.Application.Interfaces;

public class GeneratePOSEXCELReport : IGenerateReport
{
    private readonly ILogger<GeneratePOSEXCELReport> _logger;

    public GeneratePOSEXCELReport(ILogger<GeneratePOSEXCELReport> logger)
    {
        _logger = logger;
    }

    public async Task GenerateReportAsync(OrderReportDTO orderReport, WorkDayDTOOutput workDay, CancellationToken cancellationToken = default)
    {
        // Validate WorkDayId (cannot be Zero)
        
        _logger.LogInformation("\n******* GENERATING POS EXCEL REPORT ********\n");
        
        await Task.Delay(10000, cancellationToken);
        
        _logger.LogInformation("\n******* POS EXCEL REPORT CREATED ********\n");
    }

    public ReportType GetReportType() => ReportType.POS_EXCEL;
}