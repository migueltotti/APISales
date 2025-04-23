using FluentEmail.Core.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using Sales.Application.Interfaces;
using Sales.Domain.Interfaces;

namespace Sales.Application.MassTransit.GenerateReport;

public class GeneratePOSReportConsumer : IConsumer<GeneratePOSReportEvent>
{
    private readonly ILogger<GeneratePOSReportConsumer> _logger;
    private readonly IGenerateReportFactory _factory;
    private readonly ISendEmailService _emailService;

    public GeneratePOSReportConsumer(ILogger<GeneratePOSReportConsumer> logger, IGenerateReportFactory factory, ISendEmailService emailService)
    {
        _logger = logger;
        _factory = factory;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<GeneratePOSReportEvent> context)
    {
        var strategy = _factory.GetReportStrategy(context.Message.ReportType);
        
        // Generate Report
        using (var reportMemoryStream =
               await strategy.GenerateReportAsync(context.Message.OrderReport, context.Message.WorkDayReport))
        {
            var fileNameDate = context.Message.OrderReport.Date.ToString("ddMMyyyy");
            var reportDate = context.Message.OrderReport.Date.ToString("dd/MM/yyyy");

            // Set reportMemoryStream "pointer" to the start of the file (stream)
            reportMemoryStream.Position = 0;
        
            // TODO:
            // Send report by Email
            await _emailService.SendEmailAsync(
                context.Message.EmailDestination,
                $"Relatório do Dia {reportDate}",
                $"Segue o relatório solicitado pelo usuario: {context.Message.EmailDestination} \nFormato: {context.Message.ReportType.ToString()}", // get user requested report name
                    "relatorios",
                new Attachment 
                {
                    Data = reportMemoryStream,
                    ContentType = strategy.GetReportContentType(), // get specific contentType by report request type
                    Filename = $"Relatorio-{fileNameDate}{strategy.GetReportExtensionType()}" // get specific extension by report request type
                }
            );
        }
        
        _logger.LogInformation("\n****** Mensagem consumida com sucesso ******\n");
    }
}