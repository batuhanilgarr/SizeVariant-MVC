using System.Xml.Linq;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Memory;
using TireSearchMVC.Models;

namespace TireSearchMVC.Services;

public interface ITireService
{
    Task<List<TireModel>> GetBridgestoneTiresAsync();
    Task<List<TireModel>> GetLassaTiresAsync();
    Task<List<TireModel>> GetAllTiresAsync();
}

public class TireService : ITireService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TireService> _logger;
    private readonly IMemoryCache _cache;

    public TireService(HttpClient httpClient, ILogger<TireService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;
        
        // Set default headers
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "b2dvb21lZGlhOk9nb29NZWRpYTIwMjAh");
        // Many servers reject requests without a proper User-Agent or Accept header.
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TireSearchMVC/1.0 (+https://example.com)");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
    }

    public async Task<List<TireModel>> GetBridgestoneTiresAsync()
    {
        const string cacheKey = "bridgestone-tires";
        if (_cache.TryGetValue(cacheKey, out List<TireModel>? cached))
        {
            return cached;
        }

        var result = await FetchBridgestoneAsync();
        _cache.Set(cacheKey, result, TimeSpan.FromHours(4));
        return result;
    }

    private async Task<List<TireModel>> FetchBridgestoneAsync()
    {
        try
        {
            var query = "EXISTS ({{select {pk} from {CatalogVersion as cv JOIN Catalog AS c on {cv.catalog} = {c.pk}} where {cv:version} = 'Online' and {c:id} = 'brisaProductCatalog' and {pk}={BrisaSizeVariant:catalogVersion}}}) and EXISTS ({{select {pk} from {ArticleApprovalStatus as appr} where {appr:code}='approved' and {pk}={BrisaSizeVariant:BridgestoneComtr}}})";
            var url = $"https://backoffice.brisa-online.com/brisarestws/rest/brisasizevariants?brisasizevariants_subtypes=true&brisasizevariants_query={Uri.EscapeDataString(query)}&brisasizevariants_page=0&brisasizevariants_size=100000000";
            
            var response = await _httpClient.GetStringAsync(url);
            return ParseXmlResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Bridgestone tires");
            return new List<TireModel>();
        }
    }

    public async Task<List<TireModel>> GetLassaTiresAsync()
    {
        const string cacheKey = "lassa-tires";
        if (_cache.TryGetValue(cacheKey, out List<TireModel>? cached))
        {
            return cached;
        }

        var result = await FetchLassaAsync();
        _cache.Set(cacheKey, result, TimeSpan.FromHours(4));
        return result;
    }

    private async Task<List<TireModel>> FetchLassaAsync()
    {
        try
        {
            var query = "EXISTS ({{select {pk} from {CatalogVersion as cv JOIN Catalog AS c on {cv.catalog} = {c.pk}} where {cv:version} = 'Online' and {c:id} = 'brisaProductCatalog' and {pk}={BrisaSizeVariant:catalogVersion}}}) and EXISTS ({{select {pk} from {ArticleApprovalStatus as appr} where {appr:code}='approved' and {pk}={BrisaSizeVariant:LassaComtr}}})";
            var url = $"https://backoffice.brisa-online.com/brisarestws/rest/brisasizevariants?brisasizevariants_subtypes=true&brisasizevariants_query={Uri.EscapeDataString(query)}&brisasizevariants_page=0&brisasizevariants_size=100000000";
            
            var response = await _httpClient.GetStringAsync(url);
            return ParseXmlResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Lassa tires");
            return new List<TireModel>();
        }
    }

    public async Task<List<TireModel>> GetAllTiresAsync()
    {
        try
        {
            var bridgestoneTask = GetBridgestoneTiresAsync();
            var lassaTask = GetLassaTiresAsync();

            await Task.WhenAll(bridgestoneTask, lassaTask);

            var allTires = new List<TireModel>();
            allTires.AddRange(await bridgestoneTask);
            allTires.AddRange(await lassaTask);

            return allTires;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all tires");
            return new List<TireModel>();
        }
    }

    private List<TireModel> ParseXmlResponse(string xmlResponse)
    {
        try
        {
            var doc = XDocument.Parse(xmlResponse);
            var tireElements = doc.Descendants("brisasizevariant");
            
            return tireElements.Select(TireModel.FromXml).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing XML response");
            return new List<TireModel>();
        }
    }
} 