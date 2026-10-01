using PadhaiHub.Models;
using System.ComponentModel.DataAnnotations;

namespace PadhaiHub.Models;

public class Course
{
    public int CourseId { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    // Beginner, Intermediate or Advanced
    [Required, StringLength(20)]
    public string Level { get; set; } = "Beginner";

    [StringLength(255)]
    [Display(Name = "Cover image")]
    public string? ImagePath { get; set; }

    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    [Display(Name = "Created")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<DiscussionPost> DiscussionPosts { get; set; } = new List<DiscussionPost>();
}