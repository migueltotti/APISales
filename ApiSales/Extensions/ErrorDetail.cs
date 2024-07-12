using System.Text.Json;

namespace ApiSales.Extensions;

public class ErrorDetail
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public string? Trace { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}