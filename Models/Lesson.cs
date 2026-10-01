using System.ComponentModel.DataAnnotations;

namespace PadhaiHub.Models;

public class Lesson
{
    public int LessonId { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.MultilineText)]
    public string Content { get; set; } = string.Empty;

    // A YouTube "embed" link, e.g. https://www.youtube.com/embed/abc123
    [Url, StringLength(255)]
    [Display(Name = "Video URL")]
    public string? VideoUrl { get; set; }

    [Range(1, 100)]
    [Display(Name = "Lesson order")]
    public int LessonOrder { get; set; } = 1;

    [Display(Name = "Course")]
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a course.")]
    public int CourseId { get; set; }
    public Course? Course { get; set; }
}