using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TireSearchMVC.Models;

namespace TireSearchMVC.Services;

public interface IVehicleVersionService
{
    Task<List<VehicleVersionModel>> GetAllVersionsAsync();
}

public class VehicleVersionService : IVehicleVersionService
{
    private const string CacheKey = "vehicle-versions";
    private readonly HttpClient _httpClient;
    private readonly ILogger<VehicleVersionService> _logger;
    private readonly IMemoryCache _cache;

    private static readonly Uri Endpoint = new("https://backoffice.brisa-online.com/brisarestws/rest/brisavehicleversions");

    public VehicleVersionService(HttpClient httpClient, ILogger<VehicleVersionService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "b2dvb21lZGlhOk9nb29NZWRpYTIwMjAh");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TireSearchMVC/1.0 (+https://example.com)");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<VehicleVersionModel>> GetAllVersionsAsync()
    {
        if (_cache.TryGetValue(CacheKey, out List<VehicleVersionModel>? cached) && cached.Count > 0)
            return cached;

        try
        {
            var json = await _httpClient.GetStringAsync(Endpoint);
            using var doc = JsonDocument.Parse(json);
            var list = new List<VehicleVersionModel>();
            if (doc.RootElement.TryGetProperty("brisavehicleversion", out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.EnumerateArray())
                {
                    try
                    {
                        var version = VehicleVersionModel.FromJson(item);
                        if (version != null)
                            list.Add(version);
                    }
                    catch (Exception exItem)
                    {
                        _logger.LogWarning(exItem, "Vehicle version parse error - skipping item");
                    }
                }
            }
            _cache.Set(CacheKey, list, TimeSpan.FromHours(4));
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vehicle versions");
            return new List<VehicleVersionModel>();
        }
    }
}
