using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TireSearchMVC.Models;

namespace TireSearchMVC.Services;

public interface IVehicleGroupService
{
    Task<List<VehicleGroupModel>> GetAllGroupsAsync();
}

public class VehicleGroupService : IVehicleGroupService
{
    private const string CacheKey = "vehicle-groups";
    private readonly HttpClient _httpClient;
    private readonly ILogger<VehicleGroupService> _logger;
    private readonly IMemoryCache _cache;

    private static readonly Uri Endpoint = new("https://backoffice.brisa-online.com/brisarestws/rest/brisavehiclegroups");

    public VehicleGroupService(HttpClient httpClient, ILogger<VehicleGroupService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "b2dvb21lZGlhOk9nb29NZWRpYTIwMjAh");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TireSearchMVC/1.0 (+https://example.com)");
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<VehicleGroupModel>> GetAllGroupsAsync()
    {
        if (_cache.TryGetValue(CacheKey, out List<VehicleGroupModel>? cached))
        {
            return cached;
        }

        try
        {
            var json = await _httpClient.GetStringAsync(Endpoint);
            using var doc = JsonDocument.Parse(json);
            var list = new List<VehicleGroupModel>();
            if (doc.RootElement.TryGetProperty("brisavehiclegroup", out var groups) && groups.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in groups.EnumerateArray())
                {
                    list.Add(VehicleGroupModel.FromJson(item));
                }
            }
            _cache.Set(CacheKey, list, TimeSpan.FromHours(4));
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vehicle groups");
            return new List<VehicleGroupModel>();
        }
    }
}
