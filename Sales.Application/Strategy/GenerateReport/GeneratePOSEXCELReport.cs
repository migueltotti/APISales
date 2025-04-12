using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.WorkDayDTO;
using Sales.Application.Interfaces;
using Sales.Domain.Models.Enums;

namespace Sales.Application.Strategy.GenerateReport;

public class GeneratePOSEXCELReport : IGenerateReport
{
    private readonly ILogger<GeneratePOSEXCELReport> _logger;
    private readonly Dictionary<Status, string> _orderStatusInPortuguese = new()
    {
        { Status.Preparing, "Preparando" },
        { Status.Sent, "Enviado" },
        { Status.Finished, "Finalizado" }
    };

    public GeneratePOSEXCELReport(ILogger<GeneratePOSEXCELReport> logger)
    {
        _logger = logger;
    }

    public async Task<MemoryStream> GenerateReportAsync(OrderReportDTO orderReport, WorkDayDTOOutput workDay, CancellationToken cancellationToken = default)
    {
        // Validate WorkDayId (cannot be Zero)
        
        _logger.LogInformation("\n******* GENERATING POS EXCEL REPORT ********\n");

        var memoryStream = new MemoryStream();

        var reportDate = orderReport.Date.ToString("dd-MM-yyyy");
        var headerReportDate = orderReport.Date.ToString("dd/MM/yyyy");

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add($"Relatório {reportDate}");

        // Title
        ws.Range("B2:F2").Merge();
        ws.Cell("B2").Value = "Relatório de Pedidos";
        ws.Cell("B2").Style
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Font.SetBold(true)
            .Font.SetItalic(true)
            .Font.SetFontSize(15)
            .Fill.SetBackgroundColor(XLColor.Orange)
            .Border.SetBottomBorder(XLBorderStyleValues.Thin)
            .Border.SetBottomBorderColor(XLColor.Black);

        // Header
        ws.Cell("B3").Value = "Data:";
        ws.Cell("B3").Style
            .Font.SetBold(true)
            .Fill.SetBackgroundColor(XLColor.FromArgb(r: 183, g: 225, b: 205));
        ws.Cell("B4").Value = headerReportDate;
        ws.Cell("B3").Style
            .Border.SetTopBorder(XLBorderStyleValues.Thin)
            .Border.SetRightBorder(XLBorderStyleValues.Thin)
            .Border.SetTopBorderColor(XLColor.Black)
            .Border.SetRightBorderColor(XLColor.Black);
        ws.Cell("B4").Style
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Border.SetBottomBorder(XLBorderStyleValues.Thin)
            .Border.SetRightBorder(XLBorderStyleValues.Thin)
            .Border.SetBottomBorderColor(XLColor.Black)
            .Border.SetRightBorderColor(XLColor.Black);

        ws.Cell("C3").Value = "Total de Pedidos:";
        ws.Cell("C3").Style
            .Font.SetBold(true)
            .Fill.SetBackgroundColor(XLColor.FromArgb(r: 183, g: 225, b: 205));
        ws.Cell("C4").Value = orderReport.OrdersCount;
        ws.Cell("C3").Style
            .Border.SetTopBorder(XLBorderStyleValues.Thin)
            .Border.SetRightBorder(XLBorderStyleValues.Thin)
            .Border.SetTopBorderColor(XLColor.Black)
            .Border.SetRightBorderColor(XLColor.Black);
        ws.Cell("C4").Style
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Border.SetBottomBorder(XLBorderStyleValues.Thin)
            .Border.SetRightBorder(XLBorderStyleValues.Thin)
            .Border.SetBottomBorderColor(XLColor.Black)
            .Border.SetRightBorderColor(XLColor.Black);

        ws.Cell("D3").Value = "Valor Total:";
        ws.Cell("D3").Style
            .Font.SetBold(true)
            .Fill.SetBackgroundColor(XLColor.FromArgb(r: 183, g: 225, b: 205));
        ws.Cell("D4").Value = $"R$ {orderReport.TotalValue:N2}";
        ws.Cell("D3").Style
            .Border.SetTopBorder(XLBorderStyleValues.Thin)
            .Border.SetRightBorder(XLBorderStyleValues.Thin)
            .Border.SetTopBorderColor(XLColor.Black)
            .Border.SetRightBorderColor(XLColor.Black);
        ws.Cell("D4").Style
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Border.SetBottomBorder(XLBorderStyleValues.Thin)
            .Border.SetRightBorder(XLBorderStyleValues.Thin)
            .Border.SetBottomBorderColor(XLColor.Black)
            .Border.SetRightBorderColor(XLColor.Black);

        ws.Range("E3:F3").Merge();
        ws.Cell("E3").Value = "Empregado:";
        ws.Cell("E3").Style
            .Font.SetBold(true)
            .Fill.SetBackgroundColor(XLColor.FromArgb(r: 183, g: 225, b: 205));
        ws.Range("E4:F4").Merge();
        ws.Cell("E4").Value = !workDay.WorkDayId.Equals(0) ? workDay.EmployeeName : "~Empregado~";
        ws.Range("E3:F3").Style
            .Border.SetTopBorder(XLBorderStyleValues.Thin)
            .Border.SetRightBorder(XLBorderStyleValues.Thin)
            .Border.SetTopBorderColor(XLColor.Black)
            .Border.SetRightBorderColor(XLColor.Black);
        ws.Range("E4:F4").Style
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Border.SetBottomBorder(XLBorderStyleValues.Thin)
            .Border.SetRightBorder(XLBorderStyleValues.Thin)
            .Border.SetBottomBorderColor(XLColor.Black)
            .Border.SetRightBorderColor(XLColor.Black);

        // Orders
        int currentLine = 6;

        foreach (var order in orderReport.Orders)
        {
            ws.Range($"B{currentLine}:F{currentLine}").Merge();
            ws.Cell(currentLine, 2).Value = $"Pedido #{order.OrderId}";
            ws.Cell(currentLine, 2).Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.Yellow);
            ws.Range($"B{currentLine}:F{currentLine++}").Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetTopBorderColor(XLColor.Black);

            ws.Cell(currentLine, 2).Value = "Data:";
            ws.Cell(currentLine, 2).Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.LightCornflowerBlue);

            ws.Cell(currentLine, 3).Value = "Valor Total:";
            ws.Cell(currentLine, 3).Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.LightCornflowerBlue);

            ws.Cell(currentLine, 4).Value = "Status:";
            ws.Cell(currentLine, 4).Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.LightCornflowerBlue);

            ws.Cell(currentLine, 5).Value = "Titular:";
            ws.Cell(currentLine, 5).Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.LightCornflowerBlue);

            ws.Cell(currentLine, 6).Value = "Observação:";
            ws.Cell(currentLine, 6).Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.LightCornflowerBlue);

            currentLine++;

            ws.Cell(currentLine, 2).Value = order.OrderDate.ToString("dd/MM/yyyy");
            ws.Cell(currentLine, 2).Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(XLColor.LightBlue);

            ws.Cell(currentLine, 3).Value = $"R$ {order.TotalValue:N2}";
            ws.Cell(currentLine, 3).Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(XLColor.LightBlue);

            ws.Cell(currentLine, 4).Value = _orderStatusInPortuguese[order.OrderStatus];
            ws.Cell(currentLine, 4).Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(XLColor.LightBlue);

            ws.Cell(currentLine, 5).Value = order.Holder;
            ws.Cell(currentLine, 5).Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(XLColor.LightBlue);

            ws.Cell(currentLine, 6).Value = order.Note;
            ws.Cell(currentLine, 6).Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(XLColor.LightBlue);

            currentLine++;

            // Items Header
            ws.Cell(currentLine, 2).Value = "Produto:";
            ws.Cell(currentLine, 2).Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.LightCornflowerBlue);

            ws.Cell(currentLine, 3).Value = "Preço:";
            ws.Cell(currentLine, 3).Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.LightCornflowerBlue);

            ws.Cell(currentLine, 4).Value = "Quantidade:";
            ws.Cell(currentLine, 4).Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.LightCornflowerBlue);

            ws.Cell(currentLine, 5).Value = "Valor Total:";
            ws.Cell(currentLine, 5).Style
                .Font.SetBold(true)
                .Fill.SetBackgroundColor(XLColor.LightCornflowerBlue);

            // Line fill
            ws.Cell(currentLine, 6).Style
                .Fill.SetBackgroundColor(XLColor.LightCornflowerBlue);

            currentLine++;

            foreach (var item in order.LineItems ?? new())
            {
                ws.Cell(currentLine, 2).Value = item.Product.Name;
                ws.Cell(currentLine, 2).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Fill.SetBackgroundColor(XLColor.LightBlue);

                ws.Cell(currentLine, 3).Value = $"R$ {item.Price:N2}";
                ws.Cell(currentLine, 3).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Fill.SetBackgroundColor(XLColor.LightBlue);

                ws.Cell(currentLine, 4).Value = item.Amount;
                ws.Cell(currentLine, 4).Style
                    .Fill.SetBackgroundColor(XLColor.LightBlue)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell(currentLine, 5).Value = $"R$ {(item.Price * item.Amount):N2}";
                ws.Cell(currentLine, 5).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Fill.SetBackgroundColor(XLColor.LightBlue);

                // Line fill
                ws.Cell(currentLine, 6).Style
                    .Fill.SetBackgroundColor(XLColor.LightBlue);

                currentLine++;
            }

            if (!orderReport.Orders.Last().OrderId.Equals(order.OrderId))
            {
                ws.Range($"B{currentLine}:F{currentLine}").Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Border.SetTopBorderColor(XLColor.Black);
            }

            currentLine++; // Space between orders
        }

        ws.Range($"B2:F{currentLine - 2}").Style
            .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
            .Border.SetOutsideBorderColor(XLColor.Black);

        ws.Columns().AdjustToContents();
        //wb.SaveAs($@"C:\Users\migue\OneDrive\Documentos\Faculdade\ciencia_dados\relatorio_{fileTitleDate}.xlsx");
        wb.SaveAs(memoryStream);
        
        _logger.LogInformation("\n******* POS EXCEL REPORT CREATED ********\n");

        return memoryStream;
    }

    public ReportType GetReportType() => ReportType.POS_EXCEL;
}