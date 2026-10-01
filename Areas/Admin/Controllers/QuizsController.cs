using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PadhaiHub.Data;
using PadhaiHub.Models;

namespace PadhaiHub.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class QuizzesController : Controller
{
    private readonly AppDbContext _context;

    public QuizzesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Quizzes
    public async Task<IActionResult> Index()
    {
        var quizzes = _context.Quizzes
            .Include(q => q.Course)
            .Include(q => q.Questions);
        return View(await quizzes.ToListAsync());
    }

    // GET: Admin/Quizzes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var quiz = await _context.Quizzes
            .Include(q => q.Course)
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(m => m.QuizId == id);

        if (quiz == null) return NotFound();

        return View(quiz);
    }

    // GET: Admin/Quizzes/Create
    public IActionResult Create()
    {
        ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title");
        return View();
    }

    // POST: Admin/Quizzes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("QuizId,Title,Description,CourseId")] Quiz quiz)
    {
        if (ModelState.IsValid)
        {
            _context.Add(quiz);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Quiz \"{quiz.Title}\" created. Now add some questions.";
            // Redirect to Details so admin can add questions right away
            return RedirectToAction(nameof(Details), new { id = quiz.QuizId });
        }
        ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title", quiz.CourseId);
        return View(quiz);
    }

    // GET: Admin/Quizzes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var quiz = await _context.Quizzes.FindAsync(id);
        if (quiz == null) return NotFound();

        ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title", quiz.CourseId);
        return View(quiz);
    }

    // POST: Admin/Quizzes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("QuizId,Title,Description,CourseId")] Quiz quiz)
    {
        if (id != quiz.QuizId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(quiz);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Quiz updated.";
            return RedirectToAction(nameof(Index));
        }
        ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title", quiz.CourseId);
        return View(quiz);
    }

    // GET: Admin/Quizzes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var quiz = await _context.Quizzes
            .Include(q => q.Course)
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(m => m.QuizId == id);

        if (quiz == null) return NotFound();

        return View(quiz);
    }

    // POST: Admin/Quizzes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var quiz = await _context.Quizzes.FindAsync(id);
        if (quiz != null)
        {
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Quiz deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}