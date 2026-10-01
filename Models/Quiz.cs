using System.ComponentModel.DataAnnotations;

namespace PadhaiHub.Models;

public class Quiz
{
    public int QuizId { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }

    [Display(Name = "Course")]
    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}
