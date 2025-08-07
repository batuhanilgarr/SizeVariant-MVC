using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TireSearchMVC.Models;

namespace TireSearchMVC.Services;

public interface IBrandService
{
    Task<List<BrandModel>> GetAllBrandsAsync();
}

public class BrandService : IBrandService
{
    private const string CacheKey = "vehicle-brands";
    private readonly HttpClient _httpClient;
    private readonly ILogger<BrandService> _logger;
    private readonly IMemoryCache _cache;

    private static readonly Uri Endpoint = new("https://backoffice.brisa-online.com/brisarestws/rest/brisavehiclebrands");

    public BrandService(HttpClient httpClient, ILogger<BrandService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;

        // Default headers (same as TireService)
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "b2dvb21lZGlhOk9nb29NZWRpYTIwMjAh");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TireSearchMVC/1.0 (+https://example.com)");
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<BrandModel>> GetAllBrandsAsync()
    {
        if (_cache.TryGetValue(CacheKey, out List<BrandModel>? cached))
        {
            return cached;
        }

        try
        {
            var json = await _httpClient.GetStringAsync(Endpoint);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var list = new List<BrandModel>();
            if (root.TryGetProperty("brisavehiclebrand", out var brands) && brands.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in brands.EnumerateArray())
                {
                    list.Add(BrandModel.FromJson(item));
                }
            }

            _cache.Set(CacheKey, list, TimeSpan.FromHours(4));
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vehicle brands");
            return new List<BrandModel>();
        }
    }
}
