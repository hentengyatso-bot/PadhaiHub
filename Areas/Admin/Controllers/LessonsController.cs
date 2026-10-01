using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadhaiHub.Models;
using PadhaiHub.Data;

namespace PadhaiHub.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class LessonsController : Controller
{
    private readonly AppDbContext _context;

    public LessonsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Lessons
    public async Task<IActionResult> Index()
    {
        var lessons = _context.Lessons.Include(l => l.Course);
        return View(await lessons.ToListAsync());
    }

    // GET: Admin/Lessons/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var lesson = await _context.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(m => m.LessonId == id);

        if (lesson == null) return NotFound();

        return View(lesson);
    }

    // GET: Admin/Lessons/Create
    public IActionResult Create()
    {
        ViewData["CourseId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            _context.Courses, "CourseId", "Title");
        return View();
    }

    // POST: Admin/Lessons/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("LessonId,Title,Content,VideoUrl,LessonOrder,CourseId")] Lesson lesson)
    {
        if (ModelState.IsValid)
        {
            _context.Add(lesson);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Lesson created.";
            return RedirectToAction(nameof(Index));
        }
        ViewData["CourseId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            _context.Courses, "CourseId", "Title", lesson.CourseId);
        return View(lesson);
    }

    // GET: Admin/Lessons/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null) return NotFound();

        ViewData["CourseId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            _context.Courses, "CourseId", "Title", lesson.CourseId);
        return View(lesson);
    }

    // POST: Admin/Lessons/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("LessonId,Title,Content,VideoUrl,LessonOrder,CourseId")] Lesson lesson)
    {
        if (id != lesson.LessonId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(lesson);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Lesson updated.";
            return RedirectToAction(nameof(Index));
        }
        ViewData["CourseId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            _context.Courses, "CourseId", "Title", lesson.CourseId);
        return View(lesson);
    }

    // GET: Admin/Lessons/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var lesson = await _context.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(m => m.LessonId == id);

        if (lesson == null) return NotFound();

        return View(lesson);
    }

    // POST: Admin/Lessons/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson != null)
        {
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Lesson deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}