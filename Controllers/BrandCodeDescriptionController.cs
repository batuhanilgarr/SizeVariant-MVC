using Microsoft.AspNetCore.Mvc;
using TireSearchMVC.Models;
using TireSearchMVC.Services;

namespace TireSearchMVC.Controllers;

public class BrandCodeDescriptionController : Controller
{
    private readonly IBrandCodeDescriptionService _service;
    private readonly ILogger<BrandCodeDescriptionController> _logger;

    public BrandCodeDescriptionController(IBrandCodeDescriptionService service, ILogger<BrandCodeDescriptionController> logger)
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
                list = list.Where(b =>
                    (b.Code?.ToLower().Contains(term) == true) ||
                    (b.Description?.ToLower().Contains(term) == true)
                ).ToList();
            }
            list = list.OrderBy(b => b.Code).ToList();
            ViewBag.Search = search;
            return View(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading brand code descriptions");
            ViewBag.Error = "Veriler yüklenirken hata oluştu.";
            return View(new List<BrandCodeDescriptionModel>());
        }
    }
}
