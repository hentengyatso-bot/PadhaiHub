using System.ComponentModel.DataAnnotations;

namespace PadhaiHub.ViewModels;

public class CourseFormViewModel
{
    public int CourseId { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Level { get; set; } = "Beginner";

    [Display(Name = "Category")]
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a category.")]
    public int CategoryId { get; set; }

    public string? ExistingImagePath { get; set; }

    [Display(Name = "Cover image")]
    public IFormFile? ImageFile { get; set; }
}