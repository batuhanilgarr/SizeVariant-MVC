using Microsoft.AspNetCore.Mvc;
using TireSearchMVC.Models;
using TireSearchMVC.Services;

namespace TireSearchMVC.Controllers;

public class SeasonController : Controller
{
    private readonly ISeasonService _service;
    private readonly ILogger<SeasonController> _logger;

    public SeasonController(ISeasonService service, ILogger<SeasonController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string search = "")
    {
        try
        {
            var list = await _service.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                list = list.Where(s =>
                    (s.Code?.ToLower().Contains(term) == true) ||
                    (s.Description?.ToLower().Contains(term) == true)
                ).ToList();
            }
            list = list.OrderBy(s => s.Code).ToList();
            ViewBag.Search = search;
            return View(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading seasons");
            ViewBag.Error = "Veriler yüklenirken hata oluştu.";
            return View(new List<SeasonModel>());
        }
    }
}
