using Microsoft.Extensions.Logging;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.WorkDayDTO;
using Sales.Domain.Models.Enums;

namespace Sales.Application.Interfaces;

public class GeneratePOSPDFReport : IGenerateReport
{
    private readonly ILogger<GeneratePOSPDFReport> _logger;
    public async Task GenerateReportAsync(OrderReportDTO orderReport, WorkDayDTOOutput workDay, CancellationToken cancellationToken = default)
    {
        // Validate WorkDayId (cannot be Zero)
        
        _logger.LogInformation("\n******* GENERATING POS PDF REPORT ********\n");
        
        await Task.Delay(10000, cancellationToken);
        
        _logger.LogInformation("\n******* POS PDF REPORT CREATED ********\n");
    }

    public ReportType GetReportType() => ReportType.POS_PDF;
}
