using Microsoft.AspNetCore.Mvc;
using TireSearchMVC.Models;
using TireSearchMVC.Services;

namespace TireSearchMVC.Controllers;

public class TsSegmentController : Controller
{
    private readonly ITsSegmentService _service;
    private readonly ILogger<TsSegmentController> _logger;

    public TsSegmentController(ITsSegmentService service, ILogger<TsSegmentController> logger)
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
                list = list.Where(t => (t.Code?.ToLower().Contains(term) == true) || (t.Description?.ToLower().Contains(term) == true)).ToList();
            }
            list = list.OrderBy(t => t.Code).ToList();
            ViewBag.Search = search;
            return View(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading ts segments");
            ViewBag.Error = "Veriler yüklenirken hata oluştu.";
            return View(new List<TsSegmentModel>());
        }
    }
}
