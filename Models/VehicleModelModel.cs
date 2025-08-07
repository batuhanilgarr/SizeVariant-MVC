    using System.Text.Json;

namespace TireSearchMVC.Models;

public class VehicleModelModel
{
    public string? UbyId { get; set; }
    public string? BrandDescription { get; set; }
    public string? BrandUbyId { get; set; }
    public string? GroupDescription { get; set; }
    public string? GroupUbyId { get; set; }
    public DateTime? ModifiedTime { get; set; }

    public static VehicleModelModel FromJson(JsonElement element)
    {
        string? brandDesc = null, brandId = null, groupDesc = null, groupId = null;
        if (element.TryGetProperty("brand", out var brand))
        {
            brandDesc = brand.TryGetProperty("description", out var bd) ? bd.GetString() : null;
            brandId = brand.TryGetProperty("ubyId", out var bid) ? bid.GetString() : null;
        }
        if (element.TryGetProperty("group", out var group) && group.TryGetProperty("brisaVehicleGroup", out var gv))
        {
            groupDesc = gv.TryGetProperty("description", out var gd) ? gd.GetString() : null;
            groupId = gv.TryGetProperty("ubyId", out var gid) ? gid.GetString() : null;
        }

        return new VehicleModelModel
        {
            UbyId = element.GetProperty("ubyId").GetString(),
            BrandDescription = brandDesc,
            BrandUbyId = brandId,
            GroupDescription = groupDesc,
            GroupUbyId = groupId,
            ModifiedTime = element.TryGetProperty("modifiedtime", out var mt) && mt.ValueKind == JsonValueKind.String && DateTime.TryParse(mt.GetString(), out var dt) ? dt : null
        };
    }
}
