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
            // Send report by Email
            var fileNameDate = context.Message.OrderReport.Date.ToString("ddMMyyyy");
            var reportDate = context.Message.OrderReport.Date.ToString("dd/MM/yyyy");

            // Set reportMemoryStream "pointer" to the start of the file (stream)
            reportMemoryStream.Position = 0;
        
            // TODO:
            await _emailService.SendEmailAsync(
                "migueltotti2005@gmail.com", // get user requested report email
                $"Relatório do Dia {reportDate}",
                $"Segue o relatório solicitado pelo usuario: {"qalquer nome"} no formato: {context.Message.ReportType.ToString()}", // get user requested report name
                new Attachment 
                {
                    Data = reportMemoryStream,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // get specific contentType by report request type
                    Filename = $"Relatorio-{fileNameDate}.xlsx" // get specific extension by report request type
                }
            );
        }
        
        _logger.LogInformation("\n****** Mensagem consumida com sucesso ******\n");
    }
}