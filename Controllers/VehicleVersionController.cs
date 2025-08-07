using Microsoft.AspNetCore.Mvc;
using TireSearchMVC.Models;
using TireSearchMVC.Services;

namespace TireSearchMVC.Controllers;

public class VehicleVersionController : Controller
{
    private readonly IVehicleVersionService _service;
    private readonly ILogger<VehicleVersionController> _logger;

    public VehicleVersionController(IVehicleVersionService service, ILogger<VehicleVersionController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string search = "")
    {
        try
        {
            var versions = await _service.GetAllVersionsAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                versions = versions.Where(v =>
                    (v.Description?.ToLower().Contains(term) == true) ||
                    (v.UbyId == term) ||
                    (v.VersionCode == term) ||
                    (v.ModelUbyId == term)
                ).ToList();
            }
            var ordered = versions.OrderByDescending(v => v.ModifiedTime ?? DateTime.MinValue).ToList();
            ViewBag.Search = search;
            return View(ordered);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading vehicle versions");
            ViewBag.Error = "Versiyonlar yüklenirken hata oluştu.";
            return View(new List<VehicleVersionModel>());
        }
    }
}
