using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DotnetUserManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using DotnetUserManagementSystem.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DotnetUserManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<ApplicationUser> userManager;

    public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        this.userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var hi = await userManager.Users.ToListAsync();
        
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
