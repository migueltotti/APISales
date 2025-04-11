using MassTransit;
using Microsoft.Extensions.Logging;
using Sales.Application.Interfaces;
using Sales.Domain.Interfaces;

namespace Sales.Application.MassTransit.GenerateReport;

public class GeneratePOSReportConsumer : IConsumer<GeneratePOSReportEvent>
{
    private readonly ILogger<GeneratePOSReportConsumer> _logger;
    private readonly IGenerateReportFactory _factory;
    private readonly IUnitOfWork _unitOfWork;

    public GeneratePOSReportConsumer(ILogger<GeneratePOSReportConsumer> logger, IGenerateReportFactory factory, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _factory = factory;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<GeneratePOSReportEvent> context)
    {
        var strategy = _factory.GetReportStrategy(context.Message.ReportType);
        
        var report = strategy.GenerateReportAsync(context.Message.OrderReport, context.Message.WorkDayReport);
        
        _logger.LogInformation("\n****** Mensagem consumida com sucesso ******\n");
    }
}