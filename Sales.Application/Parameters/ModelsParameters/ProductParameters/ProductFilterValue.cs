namespace Sales.Application.Parameters.ModelsParameters.ProductParameters;

public class ProductFilterValue : QueryStringParameters
{
    public decimal? Price { get; set; }
    public string? PriceCriteria { get; set; } // greater ; equal ; less
}