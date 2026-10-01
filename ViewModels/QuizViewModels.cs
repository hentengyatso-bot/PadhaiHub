using PadhaiHub.Models;

namespace PadhaiHub.ViewModels;

public class TakeQuizViewModel
{
    public int QuizId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CourseId { get; set; }
    public List<Question> Questions { get; set; } = new();
}

public class QuizResultViewModel
{
    public string QuizTitle { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public int Score { get; set; }
    public int Total { get; set; }
    public int Percentage => Total == 0 ? 0 : Score * 100 / Total;
    public List<QuestionResult> Results { get; set; } = new();
}

public class QuestionResult
{
    public string QuestionText { get; set; } = string.Empty;
    public string? YourAnswer { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}