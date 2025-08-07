using System.Text.Json;

namespace TireSearchMVC.Models;

public class SeasonModel
{
    public string? Code { get; set; }
    public string? Description { get; set; }
    public DateTime? ModifiedTime { get; set; }

    public static SeasonModel FromJson(JsonElement element)
    {
        var code = element.TryGetProperty("@code", out var cp) ? cp.GetString() : element.TryGetProperty("code", out var c2) ? c2.GetString() : null;
        var desc = element.TryGetProperty("description", out var d) ? d.GetString() : null;
        DateTime? mod = null;
        if (element.TryGetProperty("modifiedtime", out var mt) && mt.ValueKind == JsonValueKind.String && DateTime.TryParse(mt.GetString(), out var dt))
            mod = dt;

        return new SeasonModel
        {
            Code = code,
            Description = desc,
            ModifiedTime = mod
        };
    }
}
