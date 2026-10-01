using PadhaiHub.Models;
using System.ComponentModel.DataAnnotations;

namespace PadhaiHub.Models;

public class DiscussionPost
{
    public int DiscussionPostId { get; set; }

    [Required(ErrorMessage = "Write something before posting.")]
    [StringLength(1000, MinimumLength = 2)]
    public string Message { get; set; } = string.Empty;

    [Display(Name = "Posted on")]
    public DateTime PostedAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }
    public User? User { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }
}