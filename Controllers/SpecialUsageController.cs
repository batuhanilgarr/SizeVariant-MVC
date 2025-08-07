using Microsoft.AspNetCore.Mvc;
using TireSearchMVC.Models;
using TireSearchMVC.Services;

namespace TireSearchMVC.Controllers;

public class SpecialUsageController : Controller
{
    private readonly ISpecialUsageService _service;
    private readonly ILogger<SpecialUsageController> _logger;

    public SpecialUsageController(ISpecialUsageService service, ILogger<SpecialUsageController> logger)
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
                list = list.Where(su => (su.Code?.ToLower().Contains(term) == true) || (su.Description?.ToLower().Contains(term) == true)).ToList();
            }
            list = list.OrderBy(su => su.Code).ToList();
            ViewBag.Search = search;
            return View(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading special usages");
            ViewBag.Error = "Veriler yüklenirken hata oluştu.";
            return View(new List<SpecialUsageModel>());
        }
    }
}
