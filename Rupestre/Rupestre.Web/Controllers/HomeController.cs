using Microsoft.AspNetCore.Mvc;
using Rupestre.Application.Interfaces;
using Rupestre.Web.Models;
using System.Diagnostics;

namespace Rupestre.Web.Controllers;

public class HomeController : Controller
{
    private readonly IDashboardService _dashboardService;

    public HomeController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var dto = await _dashboardService.GetAsync();
        return View(dto);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
