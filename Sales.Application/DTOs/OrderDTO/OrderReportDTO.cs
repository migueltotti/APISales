namespace Sales.Application.DTOs.OrderDTO;

public record OrderReportDTO
{
    public int OrdersCount { get; set; }
    public double OrdersTotalValue { get; set; }
    public int OrdersTotalProducts { get; set; }
    public DateTime OrderMinDate { get; set; }
    public DateTime OrderMaxDate { get; set; }
}