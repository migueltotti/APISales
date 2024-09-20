namespace Sales.Application.Parameters.ModelsParameters;

public class ProductParameters : QueryStringParameters
{
    public string? Name { get; set; }   
    
    public decimal? Price { get; set; }
    public string? PriceCriteria { get; set; } // greater ; equal ; less
    
    public string? TypeValue { get; set; } // un - unity ; kg - kilo
}