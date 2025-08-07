using Microsoft.AspNetCore.Mvc;
using TireSearchMVC.Models;
using TireSearchMVC.Services;

namespace TireSearchMVC.Controllers;

public class ParentGroupController : Controller
{
    private readonly IParentGroupService _service;
    private readonly ILogger<ParentGroupController> _logger;

    public ParentGroupController(IParentGroupService service, ILogger<ParentGroupController> logger)
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
                list = list.Where(p =>
                    (p.Code?.ToLower().Contains(term) == true) ||
                    (p.Description?.ToLower().Contains(term) == true)
                ).ToList();
            }
            list = list.OrderBy(p => p.Code).ToList();
            ViewBag.Search = search;
            return View(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading parent groups");
            ViewBag.Error = "Veriler yüklenirken hata oluştu.";
            return View(new List<ParentGroupModel>());
        }
    }
}
