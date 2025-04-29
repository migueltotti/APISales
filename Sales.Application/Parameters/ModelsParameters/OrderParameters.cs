namespace Sales.Application.Parameters.ModelsParameters;

public class OrderParameters : QueryStringParameters
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    
    public int? Since { get; set; }
    
    public decimal? Price { get; set; }
    public string? PriceCriteria { get; set; } // greater ; equal ; less
    
    public string? ProductName { get; set; }

    public string? Status { get; set; } = Domain.Models.Enums.Status.Preparing.ToString();
}