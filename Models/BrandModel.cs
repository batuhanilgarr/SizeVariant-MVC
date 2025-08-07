using System.Text.Json;

namespace TireSearchMVC.Models;

public class BrandModel
{
    public string? UbyId { get; set; }
    public string? Description { get; set; }
    public DateTime? ModifiedTime { get; set; }

    public static BrandModel FromJson(JsonElement element)
    {
        return new BrandModel
        {
            UbyId = element.GetProperty("ubyId").GetString(),
            Description = element.GetProperty("description").GetString(),
            ModifiedTime = element.TryGetProperty("modifiedtime", out var mt) && mt.ValueKind == JsonValueKind.String
                ? DateTime.TryParse(mt.GetString(), out var dt) ? dt : null
                : null
        };
    }
}
