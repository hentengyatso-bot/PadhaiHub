using PadhaiHub.Data;
using PadhaiHub.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PadhaiHub.Controllers;

// Public course catalogue: anyone can browse, no login needed
public class CoursesController : Controller
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Courses?search=java&categoryId=1
    public async Task<IActionResult> Index(string? search, int? categoryId)
    {
        var courses = _context.Courses.Include(c => c.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            courses = courses.Where(c =>
                c.Title.Contains(search) || c.Description.Contains(search));
        }

        if (categoryId.HasValue)
        {
            courses = courses.Where(c => c.CategoryId == categoryId.Value);
        }

        ViewData["Search"] = search;
        ViewData["CategoryId"] = new SelectList(
            await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
            "CategoryId", "Name", categoryId);

        return View(await courses.OrderBy(c => c.Title).ToListAsync());
    }

    // GET: /Courses/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Lessons)
            .Include(c => c.Quizzes)
            .Include(c => c.DiscussionPosts)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (course == null) return NotFound();

        // Is the logged-in member already enrolled?
        var isEnrolled = false;
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.GetUserId();
            isEnrolled = await _context.Enrollments.AnyAsync(e =>
                e.CourseId == id && e.UserId == userId);
        }
        ViewData["IsEnrolled"] = isEnrolled;

        return View(course);
    }
}