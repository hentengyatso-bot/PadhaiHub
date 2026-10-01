using PadhaiHub.Models;

namespace PadhaiHub.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalMembers { get; set; }
    public int TotalCourses { get; set; }
    public int TotalEnrollments { get; set; }
    public int TotalQuizAttempts { get; set; }
    public List<User> RecentMembers { get; set; } = new();
    public List<CoursePopularity> PopularCourses { get; set; } = new();
}

// Filled by a raw SQL query in the admin dashboard
public class CoursePopularity
{
    public string Title { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
}

public class MemberDashboardViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string? ProfileImagePath { get; set; }
    public List<Enrollment> Enrollments { get; set; } = new();
    public List<QuizAttempt> RecentAttempts { get; set; } = new();
    public int PostCount { get; set; }
}