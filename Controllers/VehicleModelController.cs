using Microsoft.AspNetCore.Mvc;
using TireSearchMVC.Models;
using TireSearchMVC.Services;

namespace TireSearchMVC.Controllers;

public class VehicleModelController : Controller
{
    private readonly IVehicleModelService _service;
    private readonly ILogger<VehicleModelController> _logger;

    public VehicleModelController(IVehicleModelService service, ILogger<VehicleModelController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string search = "")
    {
        try
        {
            var models = await _service.GetAllModelsAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                models = models.Where(m =>
                    (m.BrandDescription?.ToLower().Contains(term) == true) ||
                    (m.BrandUbyId == term) ||
                    (m.GroupDescription?.ToLower().Contains(term) == true) ||
                    (m.GroupUbyId == term) ||
                    (m.UbyId == term)
                ).ToList();
            }
            var ordered = models.OrderByDescending(m => m.ModifiedTime ?? DateTime.MinValue).ToList();
            ViewBag.Search = search;
            return View(ordered);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading vehicle models");
            ViewBag.Error = "Modeller yüklenirken hata oluştu.";
            return View(new List<VehicleModelModel>());
        }
    }
}
