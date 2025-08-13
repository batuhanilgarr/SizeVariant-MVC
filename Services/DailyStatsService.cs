using System.Text.Json;
using TireSearchMVC.Models;

namespace TireSearchMVC.Services;

public interface IDailyStatsService
{
    Task<StatsComparisonModel> GetStatsComparisonAsync(List<TireModel> currentTires);
    Task SaveDailyStatsAsync(List<TireModel> tires);
}

public class DailyStatsService : IDailyStatsService
{
    private readonly ILogger<DailyStatsService> _logger;
    private readonly string _statsFilePath;

    public DailyStatsService(ILogger<DailyStatsService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _statsFilePath = Path.Combine(environment.ContentRootPath, "Data", "daily-stats.json");
        
        // Ensure directory exists
        var directory = Path.GetDirectoryName(_statsFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public async Task<StatsComparisonModel> GetStatsComparisonAsync(List<TireModel> currentTires)
    {
        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);
        
        var previousStats = await LoadStatsForDateAsync(yesterday);
        var currentStats = CreateDailyStats(currentTires, today);
        
        // Get current tire codes
        var currentCodes = new HashSet<string>(currentTires
            .Where(t => !string.IsNullOrEmpty(t.Code))
            .Select(t => t.Code!)
            .Distinct());
        
        // Determine new products by comparing with previous day
        if (previousStats != null)
        {
            // Get all codes that existed on previous days (not just yesterday's new products)
            var allPreviousCodes = await GetAllPreviousCodesAsync(today);
            
            currentStats.NewProductCodes = currentCodes
                .Where(code => !allPreviousCodes.Contains(code))
                .ToList();
        }
        else
        {
            // If no previous data, don't mark anything as new (first run)
            currentStats.NewProductCodes = new List<string>();
        }

        return new StatsComparisonModel
        {
            PreviousDay = previousStats,
            CurrentDay = currentStats
        };
    }

    public async Task SaveDailyStatsAsync(List<TireModel> tires)
    {
        try
        {
            var today = DateTime.Today;
            var stats = CreateDailyStats(tires, today);
            
            // Store all current tire codes, not just new ones
            stats.NewProductCodes = tires
                .Where(t => !string.IsNullOrEmpty(t.Code))
                .Select(t => t.Code!)
                .Distinct()
                .ToList();
            
            var allStats = await LoadAllStatsAsync();
            
            // Remove existing entry for today if exists
            allStats.RemoveAll(s => s.Date.Date == today);
            
            // Add today's stats
            allStats.Add(stats);
            
            // Keep only last 30 days
            var cutoffDate = today.AddDays(-30);
            allStats = allStats.Where(s => s.Date >= cutoffDate).ToList();
            
            var json = JsonSerializer.Serialize(allStats, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_statsFilePath, json);
            
            _logger.LogInformation($"Saved daily stats for {today:yyyy-MM-dd}: {stats.TotalCount} total tires");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving daily stats");
        }
    }

    private DailyStatsModel CreateDailyStats(List<TireModel> tires, DateTime date)
    {
        return new DailyStatsModel
        {
            Date = date,
            TotalCount = tires.Count,
            BridgestoneCount = tires.Count(t => t.Brand?.Contains("BRIDGESTONE", StringComparison.OrdinalIgnoreCase) == true),
            LassaCount = tires.Count(t => t.Brand?.Contains("LASSA", StringComparison.OrdinalIgnoreCase) == true),
            NewProductCodes = new List<string>() // Will be populated in GetStatsComparisonAsync
        };
    }

    private async Task<List<DailyStatsModel>> LoadAllStatsAsync()
    {
        try
        {
            if (!File.Exists(_statsFilePath))
                return new List<DailyStatsModel>();

            var json = await File.ReadAllTextAsync(_statsFilePath);
            return JsonSerializer.Deserialize<List<DailyStatsModel>>(json) ?? new List<DailyStatsModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading stats file");
            return new List<DailyStatsModel>();
        }
    }

    private async Task<DailyStatsModel?> LoadStatsForDateAsync(DateTime date)
    {
        var allStats = await LoadAllStatsAsync();
        return allStats.FirstOrDefault(s => s.Date.Date == date.Date);
    }

    private async Task<HashSet<string>> GetAllPreviousCodesAsync(DateTime beforeDate)
    {
        var allStats = await LoadAllStatsAsync();
        var codes = new HashSet<string>();
        
        // Get all codes from all previous days (NewProductCodes field now contains all codes for that day)
        foreach (var stat in allStats.Where(s => s.Date.Date < beforeDate.Date))
        {
            foreach (var code in stat.NewProductCodes) // This now contains all codes, not just new ones
            {
                codes.Add(code);
            }
        }
        
        return codes;
    }
}
