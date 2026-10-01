using System.ComponentModel.DataAnnotations;

namespace PadhaiHub.Models;

public class User
{
    public int UserId { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    // "Admin" or "Member"
    [Required, StringLength(20)]
    public string Role { get; set; } = "Member";

    [StringLength(255)]
    [Display(Name = "Profile photo")]
    public string? ProfileImagePath { get; set; }

    [Display(Name = "Joined")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    public ICollection<DiscussionPost> DiscussionPosts { get; set; } = new List<DiscussionPost>();
}