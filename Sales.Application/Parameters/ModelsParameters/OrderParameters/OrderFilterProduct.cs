namespace Sales.Application.Parameters.ModelsParameters.OrderParameters;

public class OrderFilterProduct : QueryStringParameters
{
    public string? ProductName { get; set; }
}