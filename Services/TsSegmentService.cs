using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TireSearchMVC.Models;

namespace TireSearchMVC.Services;

public interface ITsSegmentService
{
    Task<List<TsSegmentModel>> GetAllAsync();
}

public class TsSegmentService : ITsSegmentService
{
    private const string CacheKey = "ts-segments";
    private readonly HttpClient _httpClient;
    private readonly ILogger<TsSegmentService> _logger;
    private readonly IMemoryCache _cache;

    private static readonly Uri Endpoint = new("https://backoffice.brisa-online.com/brisarestws/rest/tsegments");

    public TsSegmentService(HttpClient httpClient, ILogger<TsSegmentService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "b2dvb21lZGlhOk9nb29NZWRpYTIwMjAh");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TireSearchMVC/1.0 (+https://example.com)");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<TsSegmentModel>> GetAllAsync()
    {
        if (_cache.TryGetValue(CacheKey, out List<TsSegmentModel>? cached) && cached.Count > 0)
            return cached;

        try
        {
            var json = await _httpClient.GetStringAsync(Endpoint);
            using var doc = JsonDocument.Parse(json);
            var list = new List<TsSegmentModel>();
            if (doc.RootElement.TryGetProperty("tsegment", out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.EnumerateArray())
                {
                    try
                    {
                        list.Add(TsSegmentModel.FromJson(item));
                    }
                    catch (Exception exItem)
                    {
                        _logger.LogWarning(exItem, "Ts segment parse error - skipping item");
                    }
                }
            }
            _cache.Set(CacheKey, list, TimeSpan.FromHours(4));
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching ts segments");
            return new List<TsSegmentModel>();
        }
    }
}
