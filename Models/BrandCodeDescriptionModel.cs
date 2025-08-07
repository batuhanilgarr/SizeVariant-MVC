using System.Text.Json;

namespace TireSearchMVC.Models;

public class BrandCodeDescriptionModel
{
    public string? Code { get; set; }
    public string? Description { get; set; }

    public static BrandCodeDescriptionModel FromJson(JsonElement element)
    {
        string? code = null;
        if (element.TryGetProperty("@code", out var codeProp))
            code = codeProp.GetString();
        else if (element.TryGetProperty("code", out var codePlain))
            code = codePlain.GetString();

        var desc = element.TryGetProperty("description", out var d) ? d.GetString() : null;

        return new BrandCodeDescriptionModel
        {
            Code = code,
            Description = desc
        };
    }
}
