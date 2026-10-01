using PadhaiHub.Data;
using PadhaiHub.Helpers;
using PadhaiHub.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PadhaiHub.Controllers;

[Authorize(Roles = "Member")]
public class MemberController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public MemberController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        var model = new MemberDashboardViewModel
        {
            FullName = user.FullName,
            ProfileImagePath = user.ProfileImagePath,
            Enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync(),
            RecentAttempts = await _context.QuizAttempts
                .Include(a => a.Quiz)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AttemptedAt)
                .Take(5)
                .ToListAsync(),
            PostCount = await _context.DiscussionPosts.CountAsync(p => p.UserId == userId)
        };

        return View(model);
    }

    public async Task<IActionResult> Profile()
    {
        var user = await _context.Users.FindAsync(User.GetUserId());
        if (user == null) return RedirectToAction("Login", "Account");

        var model = new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            CurrentImagePath = user.ProfileImagePath
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        var user = await _context.Users.FindAsync(User.GetUserId());
        if (user == null) return RedirectToAction("Login", "Account");

        if (model.ProfilePhoto != null)
        {
            var error = FileHelper.ValidateImage(model.ProfilePhoto);
            if (error != null) ModelState.AddModelError(nameof(model.ProfilePhoto), error);
        }

        if (!ModelState.IsValid)
        {
            model.Email = user.Email;
            model.CurrentImagePath = user.ProfileImagePath;
            return View(model);
        }

        user.FullName = model.FullName.Trim();

        if (model.ProfilePhoto != null)
        {
            FileHelper.DeleteImage(user.ProfileImagePath, _environment.WebRootPath);
            user.ProfileImagePath = await FileHelper.SaveImageAsync(model.ProfilePhoto, _environment.WebRootPath, "profiles");
        }

        await _context.SaveChangesAsync();

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            AuthHelper.CreatePrincipal(user));

        TempData["Success"] = "Profile updated.";
        return RedirectToAction(nameof(Profile));
    }
}