using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadhaiHub.Data;

namespace PadhaiHub.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DiscussionPostsController : Controller
{
    private readonly AppDbContext _context;

    public DiscussionPostsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Admin/DiscussionPosts
    public async Task<IActionResult> Index()
    {
        var posts = await _context.DiscussionPosts
            .Include(p => p.User)
            .Include(p => p.Course)
            .OrderByDescending(p => p.PostedAt)
            .ToListAsync();
        return View(posts);
    }

    // POST: Admin/DiscussionPosts/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _context.DiscussionPosts.FindAsync(id);
        if (post != null)
        {
            _context.DiscussionPosts.Remove(post);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Post removed.";
        }
        return RedirectToAction(nameof(Index));
    }
}