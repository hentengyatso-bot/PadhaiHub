using System.ComponentModel.DataAnnotations;

namespace PadhaiHub.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required, StringLength(50)]
    [Display(Name = "Category")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}