using PadhaiHub.Data;
using PadhaiHub.Helpers;
using PadhaiHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PadhaiHub.Controllers;

[Authorize(Roles = "Member")]
public class DiscussionController : Controller
{
    private readonly AppDbContext _context;

    public DiscussionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int courseId, string message)
    {
        if (string.IsNullOrWhiteSpace(message) || message.Trim().Length < 2 || message.Length > 1000)
        {
            TempData["Error"] = "Your message must be between 2 and 1000 characters.";
            return RedirectToAction("Details", "Courses", new { id = courseId });
        }

        var userId = User.GetUserId();
        var enrolled = await _context.Enrollments.AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
        if (!enrolled)
        {
            TempData["Error"] = "Enrol in the course to join its discussion.";
            return RedirectToAction("Details", "Courses", new { id = courseId });
        }

        _context.DiscussionPosts.Add(new DiscussionPost
        {
            CourseId = courseId,
            UserId = userId,
            Message = message.Trim()
        });
        await _context.SaveChangesAsync();

        return Redirect(Url.Action("Details", "Courses", new { id = courseId }) + "#discussion");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var post = await _context.DiscussionPosts
            .FirstOrDefaultAsync(p => p.DiscussionPostId == id && p.UserId == User.GetUserId());
        if (post == null) return NotFound();

        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("DiscussionPostId,Message")] DiscussionPost input)
    {
        var post = await _context.DiscussionPosts
            .FirstOrDefaultAsync(p => p.DiscussionPostId == id && p.UserId == User.GetUserId());
        if (post == null) return NotFound();

        if (!ModelState.IsValid)
        {
            input.CourseId = post.CourseId;
            return View(input);
        }

        post.Message = input.Message.Trim();
        await _context.SaveChangesAsync();

        TempData["Success"] = "Your post has been updated.";
        return Redirect(Url.Action("Details", "Courses", new { id = post.CourseId }) + "#discussion");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _context.DiscussionPosts
            .FirstOrDefaultAsync(p => p.DiscussionPostId == id && p.UserId == User.GetUserId());
        if (post == null) return NotFound();

        var courseId = post.CourseId;
        _context.DiscussionPosts.Remove(post);
        await _context.SaveChangesAsync();

        return Redirect(Url.Action("Details", "Courses", new { id = courseId }) + "#discussion");
    }
}