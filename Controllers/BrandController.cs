using Microsoft.AspNetCore.Mvc;
using TireSearchMVC.Services;
using TireSearchMVC.Models;

namespace TireSearchMVC.Controllers;

public class BrandController : Controller
{
    private readonly IBrandService _brandService;
    private readonly ILogger<BrandController> _logger;

    public BrandController(IBrandService brandService, ILogger<BrandController> logger)
    {
        _brandService = brandService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string search = "")
    {
        try
        {
            var brands = await _brandService.GetAllBrandsAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                brands = brands.Where(b => b.Description?.ToLower().Contains(term) == true || b.UbyId == term).ToList();
            }
            ViewBag.Search = search;
            var ordered = brands.OrderByDescending(b => b.ModifiedTime ?? DateTime.MinValue).ToList();
            return View(ordered);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading brands");
            ViewBag.Error = "Markalar yüklenirken hata oluştu.";
            return View(new List<BrandModel>());
        }
    }
}
