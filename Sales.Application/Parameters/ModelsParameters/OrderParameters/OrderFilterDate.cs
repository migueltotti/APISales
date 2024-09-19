namespace Sales.Application.Parameters.ModelsParameters.OrderParameters;

public class OrderFilterDate : QueryStringParameters
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}