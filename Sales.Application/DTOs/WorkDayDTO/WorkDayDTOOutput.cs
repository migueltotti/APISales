using Sales.Application.DTOs.UserDTO;

namespace Sales.Application.DTOs.WorkDayDTO;

public record WorkDayDTOOutput(
    int WorkDayId,
    int EmployeeId,
    string? EmployeeName,
    UserDTOOutput Employee,
    DateTime StartDayTime,
    DateTime? FinishDayTime,
    int NumberOfOrders,
    int NumberOfCanceledOrders
);