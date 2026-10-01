using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadhaiHub.Data;
using PadhaiHub.Helpers;
using PadhaiHub.Models;
using PadhaiHub.ViewModels;

namespace PadhaiHub.Controllers;

[Authorize(Roles = "Member")]
public class LearnController : Controller
{
    private readonly AppDbContext _context;

    public LearnController(AppDbContext context)
    {
        _context = context;
    }

    private Task<bool> IsEnrolledAsync(int courseId)
    {
        var userId = User.GetUserId();
        return _context.Enrollments.AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
    }

    // GET: /Learn/Lesson/5
    public async Task<IActionResult> Lesson(int id)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Course)
                .ThenInclude(c => c!.Lessons)
            .FirstOrDefaultAsync(l => l.LessonId == id);

        if (lesson == null) return NotFound();

        if (!await IsEnrolledAsync(lesson.CourseId))
        {
            TempData["Error"] = "Enrol in this course to open its lessons.";
            return RedirectToAction("Details", "Courses", new { id = lesson.CourseId });
        }

        return View(lesson);
    }

    // GET: /Learn/Quiz/3
    public async Task<IActionResult> Quiz(int id)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.QuizId == id);

        if (quiz == null) return NotFound();

        if (!await IsEnrolledAsync(quiz.CourseId))
        {
            TempData["Error"] = "Enrol in this course to take its quizzes.";
            return RedirectToAction("Details", "Courses", new { id = quiz.CourseId });
        }

        var model = new TakeQuizViewModel
        {
            QuizId = quiz.QuizId,
            Title = quiz.Title,
            Description = quiz.Description,
            CourseId = quiz.CourseId,
            Questions = quiz.Questions.OrderBy(q => q.QuestionId).ToList()
        };
        return View(model);
    }

    // POST: /Learn/Quiz/3
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Quiz(int id, Dictionary<int, string> answers)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.QuizId == id);

        if (quiz == null) return NotFound();
        if (!await IsEnrolledAsync(quiz.CourseId)) return Forbid();

        var result = new QuizResultViewModel
        {
            QuizTitle = quiz.Title,
            CourseId = quiz.CourseId,
            Total = quiz.Questions.Count
        };

        foreach (var question in quiz.Questions.OrderBy(q => q.QuestionId))
        {
            answers.TryGetValue(question.QuestionId, out var chosen);
            var isCorrect = chosen == question.CorrectOption;
            if (isCorrect) result.Score++;

            result.Results.Add(new QuestionResult
            {
                QuestionText = question.QuestionText,
                YourAnswer = chosen,
                CorrectAnswer = question.CorrectOption,
                IsCorrect = isCorrect
            });
        }

        _context.QuizAttempts.Add(new QuizAttempt
        {
            QuizId = quiz.QuizId,
            UserId = User.GetUserId(),
            Score = result.Score,
            TotalQuestions = result.Total
        });
        await _context.SaveChangesAsync();

        return View("Result", result);
    }
}