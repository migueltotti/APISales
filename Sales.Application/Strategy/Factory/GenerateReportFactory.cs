using Sales.Application.Interfaces;
using Sales.Domain.Models.Enums;

namespace Sales.Application.Strategy.Factory;

public class GenerateReportFactory : IGenerateReportFactory
{
    private readonly Dictionary<ReportType, IGenerateReport> _strategies = new();

    public GenerateReportFactory(IEnumerable<IGenerateReport> strategies)
    {
        foreach (var report in strategies)
        {
            _strategies.Add(report.GetReportType(), report);
        }
    }
    
    public IGenerateReport GetReportStrategy(ReportType reportType)
    {
        if (!_strategies.TryGetValue(reportType, out IGenerateReport strategy))       
        {
            throw new ArgumentException($"No strategy defined for {reportType.ToString()}");
        }

        return strategy;
    }
}