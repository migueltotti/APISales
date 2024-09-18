namespace Sales.Application.Parameters.ModelsParameters.UserParameters;

public class UserFilterPoints : QueryStringParameters
{
    public decimal? Points { get; set; }
    public string? PointsCriteria  { get; set; }
    // greater
    // equal
    // less
}