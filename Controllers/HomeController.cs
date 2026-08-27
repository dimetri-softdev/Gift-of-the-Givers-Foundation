using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GiftOfTheGivers.Data; // Adjust if your ApplicationDbContext is in a different namespace
using GiftOfTheGivers.Models;

namespace GiftOfTheGivers.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Home/About/Privacy/Error
    [HttpGet]
        
    public async Task<IActionResult> Index()
    {
        var projects = await _context.ReliefProjects.ToListAsync();
        return View(projects);
    }
    public async Task<IActionResult> About()
    {
        // Fetch count of active relief projects for dynamic metric display
        ViewBag.ActiveProjectsCount = await _context.ReliefProjects.CountAsync();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}