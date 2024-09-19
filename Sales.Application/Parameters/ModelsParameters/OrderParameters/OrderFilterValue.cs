namespace Sales.Application.Parameters.ModelsParameters.OrderParameters;

public class OrderFilterValue : QueryStringParameters
{
    public decimal? Price { get; set; }
    public string? PriceCriteria { get; set; } // greater ; equal ; less
}