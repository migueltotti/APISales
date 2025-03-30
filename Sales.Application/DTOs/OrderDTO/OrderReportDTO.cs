namespace Sales.Application.DTOs.OrderDTO;

public record OrderReportDTO(
    DateTime Date,
    int OrdersCount,
    decimal TotalValue,
    List<OrderDTOOutput> Orders
);