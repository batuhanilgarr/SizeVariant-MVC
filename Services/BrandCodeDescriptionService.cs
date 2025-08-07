using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TireSearchMVC.Models;

namespace TireSearchMVC.Services;

public interface IBrandCodeDescriptionService
{
    Task<List<BrandCodeDescriptionModel>> GetAllAsync();
}

public class BrandCodeDescriptionService : IBrandCodeDescriptionService
{
    private const string CacheKey = "brand-code-descriptions";
    private readonly HttpClient _httpClient;
    private readonly ILogger<BrandCodeDescriptionService> _logger;
    private readonly IMemoryCache _cache;

    private static readonly Uri Endpoint = new("https://backoffice.brisa-online.com/brisarestws/rest/brisabrandcodedescriptions");

    public BrandCodeDescriptionService(HttpClient httpClient, ILogger<BrandCodeDescriptionService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "b2dvb21lZGlhOk9nb29NZWRpYTIwMjAh");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TireSearchMVC/1.0 (+https://example.com)");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<BrandCodeDescriptionModel>> GetAllAsync()
    {
        if (_cache.TryGetValue(CacheKey, out List<BrandCodeDescriptionModel>? cached) && cached.Count > 0)
            return cached;

        try
        {
            var json = await _httpClient.GetStringAsync(Endpoint);
            using var doc = JsonDocument.Parse(json);
            var list = new List<BrandCodeDescriptionModel>();
            if (doc.RootElement.TryGetProperty("brisabrandcodedescription", out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.EnumerateArray())
                {
                    try
                    {
                        var d = BrandCodeDescriptionModel.FromJson(item);
                        if (d != null)
                            list.Add(d);
                    }
                    catch (Exception exItem)
                    {
                        _logger.LogWarning(exItem, "Brand code desc parse error - skipping item");
                    }
                }
            }
            _cache.Set(CacheKey, list, TimeSpan.FromHours(4));
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching brand code descriptions");
            return new List<BrandCodeDescriptionModel>();
        }
    }
}
