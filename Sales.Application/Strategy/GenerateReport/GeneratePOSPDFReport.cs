using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.WorkDayDTO;
using Sales.Domain.Models.Enums;

namespace Sales.Application.Strategy.GenerateReport;

public class GeneratePOSPDFReport : IGenerateReport
{
    private readonly ILogger<GeneratePOSPDFReport> _logger;

    public GeneratePOSPDFReport(ILogger<GeneratePOSPDFReport> logger)
    {
        _logger = logger;
    }

    public async Task<MemoryStream> GenerateReportAsync(OrderReportDTO orderReport, WorkDayDTOOutput workDay, CancellationToken cancellationToken = default)
    {
        // Validate WorkDayId (cannot be Zero)
        
        var memoryStream = new MemoryStream();
        
        _logger.LogInformation("\n******* GENERATING POS PDF REPORT ********\n");

        var pdfGenerator = new PdfGenerator(orderReport, workDay);
        pdfGenerator.GeneratePdf(memoryStream);
        
        _logger.LogInformation("\n******* POS PDF REPORT CREATED ********\n");
        
        return memoryStream;
    }

    public ReportType GetReportType() => ReportType.POS_PDF;
    public string GetReportContentType() => "application/pdf";
    public string GetReportExtensionType() => ".pdf";
}

public class PdfGenerator: IDocument
{
    private readonly OrderReportDTO _orderReport;
    private readonly WorkDayDTOOutput _workDay;
    private readonly Dictionary<Status, string> _orderStatusInPortuguese = new()
    {
        { Status.Preparing, "Preparando" },
        { Status.Sent, "Enviado" },
        { Status.Finished, "Finalizado" }
    };
    
    public PdfGenerator(OrderReportDTO orderReport, WorkDayDTOOutput workDay)
    {
        this._workDay = workDay;
        this._orderReport = orderReport;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.Size(PageSizes.A4.Landscape());

            page.Header().Background(Colors.Orange.Medium).AlignCenter().Text("Relatório de Pedidos")
                .FontColor(Colors.White).FontSize(20).Bold();

            page.Content().Column(col =>
            {
                col.Spacing(15);

                // Tabela Resumo
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(120);
                        columns.ConstantColumn(120);
                        columns.ConstantColumn(120);
                        columns.RelativeColumn();
                    });

                    table.Cell().Element(CellStyle(true)).Text("Data:");
                    table.Cell().Element(CellStyle()).Text(_orderReport.Date.ToString("dd/MM/yyyy"));
                    table.Cell().Element(CellStyle(true)).Text("Total de Pedidos:");
                    table.Cell().Element(CellStyle()).Text(_orderReport.OrdersCount);

                    table.Cell().Element(CellStyle(true)).Text("Valor Total:");
                    table.Cell().Element(CellStyle()).Text($"R$ {_orderReport.TotalValue:N2}");
                    table.Cell().Element(CellStyle(true)).Text("Empregado:");
                    table.Cell().Element(CellStyle()).Text(_workDay.EmployeeName);
                });

                col.Item().PaddingBottom(10);

                // Tabelas de Pedidos
                foreach (var order in _orderReport.Orders)
                {
                    col.Item().Background(Colors.Yellow.Lighten3).Padding(5)
                        .Text($"Pedido #{order.OrderId}").FontSize(13).Bold();
                    
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            //columns.ConstantColumn(170);
                            columns.ConstantColumn(150);
                            columns.ConstantColumn(150);
                            columns.ConstantColumn(150);
                            columns.RelativeColumn();
                        });

                        // order header
                        //table.Cell().Element(CellStyle(true)).Text("Data:");
                        table.Cell().Element(CellStyle(true)).Text("Valor Total:");
                        table.Cell().Element(CellStyle(true)).Text("Status:");
                        table.Cell().Element(CellStyle(true)).Text("Titular:");
                        table.Cell().Element(CellStyle(true)).Text("Observação:");
                        
                        // order header data
                        //table.Cell().Element(CellStyle()).Text(order.OrderDate);
                        table.Cell().Element(CellStyle()).Text(order.TotalValue);
                        table.Cell().Element(CellStyle()).Text(_orderStatusInPortuguese[order.OrderStatus]);
                        table.Cell().Element(CellStyle()).Text(order.Holder);
                        table.Cell().Element(CellStyle()).Text(order.Note);
                        
                        // order product description
                        foreach (var item in order.LineItems ?? [])
                        {
                            // product header
                            table.Cell().Element(CellStyle(true)).Text("Produto:");
                            table.Cell().Element(CellStyle(true)).Text("Preço:");
                            table.Cell().Element(CellStyle(true)).Text("Quantidade:");
                            table.Cell().Element(CellStyle(true)).Text("Valor Total:");

                            table.Cell().Element(CellStyle()).Text(item.Product.Name);
                            table.Cell().Element(CellStyle()).Text(item.Price);
                            table.Cell().Element(CellStyle()).Text(item.Amount);
                            table.Cell().Element(CellStyle()).Text($"R$ {item.Price * item.Amount:N2}");
                        }
                    });
                }
            });

            page.Footer().AlignCenter().Text(txt =>
            {
                txt.Span("Página ").FontSize(10);
                txt.CurrentPageNumber().FontSize(10);
                txt.Span(" de ").FontSize(10);
                txt.TotalPages().FontSize(10);
            });
        });
    }

    private static Func<IContainer, IContainer> CellStyle(bool header = false) => container =>
        container
            .Border(0.5f)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5)
            .Background(header ? Colors.Grey.Lighten3 : Colors.White)
            .AlignCenter()
            .AlignMiddle();
}
