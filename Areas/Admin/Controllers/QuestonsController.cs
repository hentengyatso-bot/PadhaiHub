using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadhaiHub.Data;
using PadhaiHub.Models;

namespace PadhaiHub.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class QuestionsController : Controller
{
    private readonly AppDbContext _context;

    public QuestionsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Questions/Create?quizId=3
    public async Task<IActionResult> Create(int quizId)
    {
        var quiz = await _context.Quizzes.FindAsync(quizId);
        if (quiz == null) return NotFound();

        ViewData["QuizTitle"] = quiz.Title;
        return View(new Question { QuizId = quizId });
    }

    // POST: Admin/Questions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("QuizId,QuestionText,OptionA,OptionB,OptionC,OptionD,CorrectOption")] Question question)
    {
        if (!ModelState.IsValid)
        {
            ViewData["QuizTitle"] = (await _context.Quizzes.FindAsync(question.QuizId))?.Title;
            return View(question);
        }

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Question added.";
        return RedirectToAction("Details", "Quizzes", new { id = question.QuizId });
    }

    // GET: Admin/Questions/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var question = await _context.Questions
            .Include(q => q.Quiz)
            .FirstOrDefaultAsync(q => q.QuestionId == id);

        if (question == null) return NotFound();

        ViewData["QuizTitle"] = question.Quiz?.Title;
        return View(question);
    }

    // POST: Admin/Questions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("QuestionId,QuizId,QuestionText,OptionA,OptionB,OptionC,OptionD,CorrectOption")] Question question)
    {
        if (id != question.QuestionId) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["QuizTitle"] = (await _context.Quizzes.FindAsync(question.QuizId))?.Title;
            return View(question);
        }

        _context.Update(question);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Question updated.";
        return RedirectToAction("Details", "Quizzes", new { id = question.QuizId });
    }

    // POST: Admin/Questions/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question == null) return NotFound();

        var quizId = question.QuizId;
        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Question deleted.";
        return RedirectToAction("Details", "Quizzes", new { id = quizId });
    }
}