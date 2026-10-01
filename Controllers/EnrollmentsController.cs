using PadhaiHub.Data;
using PadhaiHub.Helpers;
using PadhaiHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PadhaiHub.Controllers;

[Authorize(Roles = "Member")]
public class EnrollmentsController : Controller
{
    private readonly AppDbContext _context;

    public EnrollmentsController(AppDbContext context)
    {
        _context = context;
    }

    // POST: /Enrollments/Enrol   (INSERT)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enrol(int courseId)
    {
        var userId = User.GetUserId();

        var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == courseId);
        if (!courseExists) return NotFound();

        var alreadyEnrolled = await _context.Enrollments
            .AnyAsync(e => e.CourseId == courseId && e.UserId == userId);

        if (!alreadyEnrolled)
        {
            _context.Enrollments.Add(new Enrollment { CourseId = courseId, UserId = userId });
            await _context.SaveChangesAsync();
            TempData["Success"] = "You are now enrolled. Start with the first lesson below.";
        }

        return RedirectToAction("Details", "Courses", new { id = courseId });
    }

    // POST: /Enrollments/ToggleComplete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleComplete(int id)
    {
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.EnrollmentId == id && e.UserId == User.GetUserId());
        if (enrollment == null) return NotFound();

        enrollment.IsCompleted = !enrollment.IsCompleted;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Member");
    }

    // POST: /Enrollments/Unenrol/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unenrol(int id)
    {
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.EnrollmentId == id && e.UserId == User.GetUserId());
        if (enrollment == null) return NotFound();

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "You have left the course.";
        return RedirectToAction("Index", "Member");
    }
}