namespace Sales.Domain.Models;

public class WorkDay
{
    public int WorkDayId { get; private set; }
    public int EmployeeId { get; private set; }
    public string? EmployeeName { get; private set; }
    public User Employee { get; private set; }
    public DateTime StartDayTime { get; private set; }
    public DateTime? FinishDayTime { get; private set; }
    public int NumberOfOrders { get; private set; }
    public int NumberOfCanceledOrders { get; private set; }

    private WorkDay() { }
    
    public WorkDay(int employeeId, string? employeeName, DateTime startDayTime, int numberOfOrders, int numberOfCanceledOrders)
    {
        EmployeeId = employeeId;
        EmployeeName = employeeName;
        StartDayTime = startDayTime;
        NumberOfOrders = numberOfOrders;
        NumberOfCanceledOrders = numberOfCanceledOrders;
    }

    public WorkDay(int workDayId, int employeeId, string? employeeName, DateTime startDayTime, DateTime finishDayTime, int numberOfOrders, int numberOfCanceledOrders)
    {
        WorkDayId = workDayId;
        EmployeeId = employeeId;
        EmployeeName = employeeName;
        StartDayTime = startDayTime;
        FinishDayTime = finishDayTime;
        NumberOfOrders = numberOfOrders;
        NumberOfCanceledOrders = numberOfCanceledOrders;
    }

    public void FinishWorkDay(int employeeId)
    {
        if(employeeId != EmployeeId)
            throw new Exception("Invalid employee id");

        FinishDayTime = DateTime.Now;
    }

    public void IncreaseNumberOfOrders()
    {
        NumberOfOrders++;
    }
    
    public void DecreaseNumberOfOrders()
    {
        if(NumberOfOrders == 0)
            throw new Exception("Number of orders cannot be zero");
        
        NumberOfOrders--;
    }
    
    public void IncreaseNumberOfCanceledOrders()
    {
        NumberOfCanceledOrders++;
    }
}