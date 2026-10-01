using System.ComponentModel.DataAnnotations;

namespace PadhaiHub.Models;

public class QuizAttempt
{
    public int QuizAttemptId { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int QuizId { get; set; }
    public Quiz? Quiz { get; set; }

    public int Score { get; set; }

    [Display(Name = "Questions")]
    public int TotalQuestions { get; set; }

    [Display(Name = "Attempted on")]
    public DateTime AttemptedAt { get; set; } = DateTime.Now;
}