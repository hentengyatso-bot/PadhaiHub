using System.ComponentModel.DataAnnotations;

namespace PadhaiHub.Models;

public class Enrollment
{
    public int EnrollmentId { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    [Display(Name = "Enrolled on")]
    public DateTime EnrolledAt { get; set; } = DateTime.Now;

    [Display(Name = "Completed")]
    public bool IsCompleted { get; set; }
}