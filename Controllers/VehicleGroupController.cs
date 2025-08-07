using Microsoft.AspNetCore.Mvc;
using TireSearchMVC.Services;
using TireSearchMVC.Models;

namespace TireSearchMVC.Controllers;

public class VehicleGroupController : Controller
{
    private readonly IVehicleGroupService _service;
    private readonly ILogger<VehicleGroupController> _logger;

    public VehicleGroupController(IVehicleGroupService service, ILogger<VehicleGroupController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string search = "")
    {
        try
        {
            var groups = await _service.GetAllGroupsAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                groups = groups.Where(g => g.Description?.ToLower().Contains(term) == true || g.UbyId == term).ToList();
            }
            var ordered = groups.OrderByDescending(g => g.ModifiedTime ?? DateTime.MinValue).ToList();
            ViewBag.Search = search;
            return View(ordered);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading vehicle groups");
            ViewBag.Error = "Gruplar yüklenirken hata oluştu.";
            return View(new List<VehicleGroupModel>());
        }
    }
}
