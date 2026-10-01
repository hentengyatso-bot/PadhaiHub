using PadhaiHub.Data;
using PadhaiHub.Helpers;
using PadhaiHub.Models;
using PadhaiHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PadhaiHub.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CoursesController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public CoursesController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    private static readonly string[] Levels = { "Beginner", "Intermediate", "Advanced" };

    // Fills the two drop-down lists on the Create and Edit forms
    private async Task LoadDropDownsAsync(int? selectedCategoryId = null, string? selectedLevel = null)
    {
        ViewData["CategoryId"] = new SelectList(
            await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
            "CategoryId", "Name", selectedCategoryId);

        ViewData["Level"] = new SelectList(Levels, selectedLevel);
    }

    // GET: Admin/Courses
    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Enrollments)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return View(courses);
    }

    // GET: Admin/Courses/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Lessons)
            .Include(c => c.Quizzes)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (course == null) return NotFound();

        return View(course);
    }

    // GET: Admin/Courses/Create
    public async Task<IActionResult> Create()
    {
        await LoadDropDownsAsync();
        return View(new CourseFormViewModel());
    }

    // POST: Admin/Courses/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseFormViewModel model)
    {
        if (model.ImageFile != null)
        {
            var error = FileHelper.ValidateImage(model.ImageFile);
            if (error != null) ModelState.AddModelError(nameof(model.ImageFile), error);
        }

        if (!ModelState.IsValid)
        {
            await LoadDropDownsAsync(model.CategoryId, model.Level);
            return View(model);
        }

        var course = new Course
        {
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            Level = model.Level,
            CategoryId = model.CategoryId
        };

        if (model.ImageFile != null)
        {
            course.ImagePath = await FileHelper.SaveImageAsync(
                model.ImageFile, _environment.WebRootPath, "courses");
        }

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Course \"{course.Title}\" created.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Courses/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        var model = new CourseFormViewModel
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            Level = course.Level,
            CategoryId = course.CategoryId,
            ExistingImagePath = course.ImagePath
        };

        await LoadDropDownsAsync(course.CategoryId, course.Level);
        return View(model);
    }

    // POST: Admin/Courses/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseFormViewModel model)
    {
        if (id != model.CourseId) return NotFound();

        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        if (model.ImageFile != null)
        {
            var error = FileHelper.ValidateImage(model.ImageFile);
            if (error != null) ModelState.AddModelError(nameof(model.ImageFile), error);
        }

        if (!ModelState.IsValid)
        {
            model.ExistingImagePath = course.ImagePath;
            await LoadDropDownsAsync(model.CategoryId, model.Level);
            return View(model);
        }

        course.Title = model.Title.Trim();
        course.Description = model.Description.Trim();
        course.Level = model.Level;
        course.CategoryId = model.CategoryId;

        if (model.ImageFile != null)
        {
            FileHelper.DeleteImage(course.ImagePath, _environment.WebRootPath);
            course.ImagePath = await FileHelper.SaveImageAsync(
                model.ImageFile, _environment.WebRootPath, "courses");
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Course updated.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Courses/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (course == null) return NotFound();

        return View(course);
    }

    // POST: Admin/Courses/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            FileHelper.DeleteImage(course.ImagePath, _environment.WebRootPath);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Course deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}