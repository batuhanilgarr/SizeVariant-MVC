using Microsoft.AspNetCore.Mvc;
using TireSearchMVC.Models;
using TireSearchMVC.Services;

namespace TireSearchMVC.Controllers;

public class SizeVariantController : Controller
{
    private readonly ITireService _tireService;
    private readonly IDailyStatsService _dailyStatsService;
    private readonly ILogger<SizeVariantController> _logger;

    public SizeVariantController(ITireService tireService, IDailyStatsService dailyStatsService, ILogger<SizeVariantController> logger)
    {
        _tireService = tireService;
        _dailyStatsService = dailyStatsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string searchTerm = "", string tab = "all")
    {
        try
        {
            var bridgestoneTask = _tireService.GetBridgestoneTiresAsync();
            var lassaTask = _tireService.GetLassaTiresAsync();
            await Task.WhenAll(bridgestoneTask, lassaTask);

            var allTires = bridgestoneTask.Result.Concat(lassaTask.Result).ToList();

            // Save daily stats (async, don't wait)
            _ = Task.Run(async () => await _dailyStatsService.SaveDailyStatsAsync(allTires));

            // Get stats comparison
            var statsComparison = await _dailyStatsService.GetStatsComparisonAsync(allTires);

            var filteredTires = tab switch
            {
                "bridgestone" => allTires.Where(t => t.Brand?.Contains("BRIDGESTONE", StringComparison.OrdinalIgnoreCase) == true).ToList(),
                "lassa" => allTires.Where(t => t.Brand?.Contains("LASSA", StringComparison.OrdinalIgnoreCase) == true).ToList(),
                _ => allTires
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Split search term by comma and trim whitespace
                var searchTerms = searchTerm.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(term => term.Trim().ToLower())
                                          .Where(term => !string.IsNullOrEmpty(term))
                                          .ToList();

                if (searchTerms.Any())
                {
                    filteredTires = filteredTires.Where(t =>
                        searchTerms.Any(term =>
                            (t.Code?.ToLower().Contains(term) == true) ||
                            (t.Name?.ToLower().Contains(term) == true) ||
                            (t.Dimension?.ToLower().Contains(term) == true) ||
                            (t.PatternDescription?.ToLower().Contains(term) == true) ||
                            (t.PatternString?.ToLower().Contains(term) == true) ||
                            (t.TechnicalGrouping?.ToLower().Contains(term) == true) ||
                            (t.Brand?.ToLower().Contains(term) == true) ||
                            (t.Season?.ToLower().Contains(term) == true) ||
                            (t.Size?.ToLower().Contains(term) == true)
                        )
                    ).ToList();
                }
            }

            // Mark new products and sort with newest first
            var newProductCodes = new HashSet<string>(statsComparison.CurrentDay.NewProductCodes);
            foreach (var tire in filteredTires)
            {
                if (!string.IsNullOrEmpty(tire.Code) && newProductCodes.Contains(tire.Code))
                {
                    // Add a property to mark as new (we'll use ViewBag for this)
                }
            }

            var ordered = filteredTires.OrderByDescending(t =>
            {
                // First, prioritize new products
                if (!string.IsNullOrEmpty(t.Code) && newProductCodes.Contains(t.Code))
                    return DateTime.MaxValue;
                
                // Then by modified time
                return DateTime.TryParse(t.ModifiedTime, out var dt) ? dt : DateTime.MinValue;
            }).ToList();

            var limitedTires = ordered.Take(20).ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentTab = tab;
            ViewBag.TotalCount = filteredTires.Count;
            ViewBag.DisplayedCount = limitedTires.Count;
            ViewBag.StatsComparison = statsComparison;
            ViewBag.NewProductCodes = newProductCodes;

            return View(limitedTires);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading size variants");
            ViewBag.Error = "Veriler yüklenirken bir hata oluştu.";
            return View(new List<TireModel>());
        }
    }

    [HttpPost]
    public IActionResult Search(string searchTerm)
    {
        return RedirectToAction("Index", new { searchTerm });
    }

    [HttpGet]
    [Route("api/tire/{code}")]
    public async Task<IActionResult> GetTireDetails(string code)
    {
        try
        {
            var allTires = await _tireService.GetAllTiresAsync();
            var tire = allTires.FirstOrDefault(t => t.Code == code);
            
            if (tire == null)
            {
                return NotFound($"Tire with code {code} not found");
            }

            return Json(tire);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tire details for code {Code}", code);
            return StatusCode(500, "Internal server error");
        }
    }
}
