using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadhaiHub.Data;
using PadhaiHub.Models;

namespace PadhaiHub.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Categories
    public async Task<IActionResult> Index()
    {
        return View(await _context.Categories.Include(c => c.Courses).ToListAsync());
    }

    // GET: Admin/Categories/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var category = await _context.Categories
            .Include(c => c.Courses)
            .FirstOrDefaultAsync(m => m.CategoryId == id);

        if (category == null) return NotFound();

        return View(category);
    }

    // GET: Admin/Categories/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CategoryId,Name,Description")] Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Category \"{category.Name}\" created.";
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: Admin/Categories/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        return View(category);
    }

    // POST: Admin/Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name,Description")] Category category)
    {
        if (id != category.CategoryId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Categories.AnyAsync(c => c.CategoryId == id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: Admin/Categories/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var category = await _context.Categories
            .Include(c => c.Courses)
            .FirstOrDefaultAsync(m => m.CategoryId == id);

        if (category == null) return NotFound();

        return View(category);
    }

    // POST: Admin/Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Refuse to delete a category that still has courses
        if (await _context.Courses.AnyAsync(c => c.CategoryId == id))
        {
            TempData["Error"] = "This category still has courses. Move or delete those courses first.";
            return RedirectToAction(nameof(Index));
        }

        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Category deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}