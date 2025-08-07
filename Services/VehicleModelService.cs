using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TireSearchMVC.Models;

namespace TireSearchMVC.Services;

public interface IVehicleModelService
{
    Task<List<VehicleModelModel>> GetAllModelsAsync();
}

public class VehicleModelService : IVehicleModelService
{
    private const string CacheKey = "vehicle-models";
    private readonly HttpClient _httpClient;
    private readonly ILogger<VehicleModelService> _logger;
    private readonly IMemoryCache _cache;

    private static readonly Uri Endpoint = new("https://backoffice.brisa-online.com/brisarestws/rest/brisavehiclemodels");

    public VehicleModelService(HttpClient httpClient, ILogger<VehicleModelService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "b2dvb21lZGlhOk9nb29NZWRpYTIwMjAh");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TireSearchMVC/1.0 (+https://example.com)");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<VehicleModelModel>> GetAllModelsAsync()
    {
        if (_cache.TryGetValue(CacheKey, out List<VehicleModelModel>? cached) && cached.Count > 0)
            return cached;

        try
        {
            var json = await _httpClient.GetStringAsync(Endpoint);
            using var doc = JsonDocument.Parse(json);
            var list = new List<VehicleModelModel>();
            if (doc.RootElement.TryGetProperty("brisavehiclemodel", out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.EnumerateArray())
                {
                    try
                    {
                        var model = VehicleModelModel.FromJson(item);
                        if (model != null)
                            list.Add(model);
                    }
                    catch (Exception exItem)
                    {
                        _logger.LogWarning(exItem, "Vehicle model parse error - skipping item");
                    }
                }
            }
            _cache.Set(CacheKey, list, TimeSpan.FromHours(4));
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vehicle models");
            return new List<VehicleModelModel>();
        }
    }
}
