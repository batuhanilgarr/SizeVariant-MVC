using System.Text.Json;

namespace TireSearchMVC.Models;

public class VehicleVersionModel
{
    public string? UbyId { get; set; }
    public string? VersionCode { get; set; }
    public string? Description { get; set; }
    public string? ModelUbyId { get; set; }
    public DateTime? ModifiedTime { get; set; }

    public static VehicleVersionModel FromJson(JsonElement element)
    {
        string? modelId = null;
        if (element.TryGetProperty("model", out var modelElem))
        {
            modelId = modelElem.TryGetProperty("ubyId", out var mid) ? mid.GetString() : null;
        }
        return new VehicleVersionModel
        {
            UbyId = element.TryGetProperty("ubyId", out var id) ? id.GetString() : null,
            VersionCode = element.TryGetProperty("versionCode", out var vc) ? vc.GetString() : null,
            Description = element.TryGetProperty("description", out var desc) ? desc.GetString() : null,
            ModelUbyId = modelId,
            ModifiedTime = element.TryGetProperty("modifiedtime", out var mt) && mt.ValueKind == JsonValueKind.String && DateTime.TryParse(mt.GetString(), out var dt) ? dt : null
        };
    }
}
