using System.ComponentModel.DataAnnotations;

namespace PadhaiHub.ViewModels;

public class ProfileViewModel
{
    [Required(ErrorMessage = "Please enter your full name.")]
    [StringLength(100, MinimumLength = 3)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? CurrentImagePath { get; set; }

    [Display(Name = "New profile photo")]
    public IFormFile? ProfilePhoto { get; set; }
}