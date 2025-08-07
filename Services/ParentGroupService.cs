using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TireSearchMVC.Models;

namespace TireSearchMVC.Services;

public interface IParentGroupService
{
    Task<List<ParentGroupModel>> GetAllAsync();
}

public class ParentGroupService : IParentGroupService
{
    private const string CacheKey = "parent-groups";
    private readonly HttpClient _httpClient;
    private readonly ILogger<ParentGroupService> _logger;
    private readonly IMemoryCache _cache;

    private static readonly Uri Endpoint = new("https://backoffice.brisa-online.com/brisarestws/rest/brisapcmparentgroups");

    public ParentGroupService(HttpClient httpClient, ILogger<ParentGroupService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "b2dvb21lZGlhOk9nb29NZWRpYTIwMjAh");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TireSearchMVC/1.0 (+https://example.com)");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<ParentGroupModel>> GetAllAsync()
    {
        if (_cache.TryGetValue(CacheKey, out List<ParentGroupModel>? cached) && cached.Count > 0)
            return cached;

        try
        {
            var json = await _httpClient.GetStringAsync(Endpoint);
            using var doc = JsonDocument.Parse(json);
            var list = new List<ParentGroupModel>();
            if (doc.RootElement.TryGetProperty("brisapcmparentgroup", out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.EnumerateArray())
                {
                    try
                    {
                        list.Add(ParentGroupModel.FromJson(item));
                    }
                    catch (Exception exItem)
                    {
                        _logger.LogWarning(exItem, "Parent group parse error - skipping item");
                    }
                }
            }
            _cache.Set(CacheKey, list, TimeSpan.FromHours(4));
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching parent groups");
            return new List<ParentGroupModel>();
        }
    }
}
