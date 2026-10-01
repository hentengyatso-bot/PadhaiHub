using PadhaiHub.Data;
using PadhaiHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PadhaiHub.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Admin  or  /Admin/Dashboard
    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel
        {
            TotalMembers = await _context.Users.CountAsync(u => u.Role == "Member"),
            TotalCourses = await _context.Courses.CountAsync(),
            TotalEnrollments = await _context.Enrollments.CountAsync(),
            TotalQuizAttempts = await _context.QuizAttempts.CountAsync(),
            RecentMembers = await _context.Users
                .Where(u => u.Role == "Member")
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync(),

            // Hand-written SQL — good for the report
            PopularCourses = await _context.Database.SqlQuery<CoursePopularity>($"""
                SELECT TOP 5 c.Title, COUNT(e.EnrollmentId) AS EnrollmentCount
                FROM Courses c
                LEFT JOIN Enrollments e ON e.CourseId = c.CourseId
                GROUP BY c.CourseId, c.Title
                ORDER BY EnrollmentCount DESC
                """).ToListAsync()
        };

        return View(model);
    }
}