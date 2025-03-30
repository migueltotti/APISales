namespace Sales.Application.DTOs.OrderDTO;

public record OrderWeekReportDTO(
    DateOnly date,
    int numberOfOrders
);