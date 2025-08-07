using System.Text.Json;

namespace TireSearchMVC.Models;

public class TsSegmentModel
{
    public string? Code { get; set; }
    public string? Description { get; set; }

    public static TsSegmentModel FromJson(JsonElement element)
    {
        var code = element.TryGetProperty("@code", out var cp) ? cp.GetString() : element.TryGetProperty("code", out var c2) ? c2.GetString() : null;
        var desc = element.TryGetProperty("description", out var d) ? d.GetString() : null;
        return new TsSegmentModel { Code = code, Description = desc };
    }
}
