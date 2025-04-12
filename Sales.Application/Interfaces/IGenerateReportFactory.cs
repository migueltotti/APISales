using Sales.Application.Strategy.GenerateReport;
using Sales.Domain.Models.Enums;

namespace Sales.Application.Interfaces;

public interface IGenerateReportFactory
{
    IGenerateReport GetReportStrategy(ReportType reportType);
}