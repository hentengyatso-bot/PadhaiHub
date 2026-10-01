using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadhaiHub.Data;
using PadhaiHub.Helpers;

namespace PadhaiHub.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public UsersController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // GET: Admin/Users?search=ali
    public async Task<IActionResult> Index(string? search)
    {
        var users = _context.Users.Include(u => u.Enrollments).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            users = users.Where(u =>
                u.FullName.Contains(search) || u.Email.Contains(search));
        }

        ViewData["Search"] = search;
        return View(await users
            .OrderBy(u => u.Role)
            .ThenBy(u => u.FullName)
            .ToListAsync());
    }

    // POST: Admin/Users/ChangeRole/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(int id, string role)
    {
        if (role != "Admin" && role != "Member") return BadRequest();

        if (id == User.GetUserId())
        {
            TempData["Error"] = "You cannot change your own role.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Role = role;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"{user.FullName} is now {role}.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Users/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (id == User.GetUserId())
        {
            TempData["Error"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        FileHelper.DeleteImage(user.ProfileImagePath, _environment.WebRootPath);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        TempData["Success"] = "User deleted.";
        return RedirectToAction(nameof(Index));
    }
}