using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadhaiHub.Data;
using PadhaiHub.Models;
using PadhaiHUb.Models;
using System.Diagnostics;

namespace PadhaiHub.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // The three newest courses for the home page
        var latestCourses = await _context.Courses
            .Include(c => c.Category)
            .OrderByDescending(c => c.CreatedAt)
            .Take(3)
            .ToListAsync();

        return View(latestCourses);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Terms()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}