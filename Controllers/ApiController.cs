using Microsoft.AspNetCore.Mvc;
using TireSearchMVC.Models;
using TireSearchMVC.Services;

namespace TireSearchMVC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TireController : ControllerBase
{
    private readonly ITireService _tireService;
    private readonly ILogger<TireController> _logger;

    public TireController(ITireService tireService, ILogger<TireController> logger)
    {
        _tireService = tireService;
        _logger = logger;
    }

    [HttpGet("bridgestone")]
    public async Task<ActionResult<List<TireModel>>> GetBridgestoneTires()
    {
        try
        {
            var tires = await _tireService.GetBridgestoneTiresAsync();
            return Ok(tires);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Bridgestone tires");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("lassa")]
    public async Task<ActionResult<List<TireModel>>> GetLassaTires()
    {
        try
        {
            var tires = await _tireService.GetLassaTiresAsync();
            return Ok(tires);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Lassa tires");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<TireModel>>> GetAllTires()
    {
        try
        {
            var tires = await _tireService.GetAllTiresAsync();
            return Ok(tires);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tires");
            return StatusCode(500, "Internal server error");
        }
    }
} 