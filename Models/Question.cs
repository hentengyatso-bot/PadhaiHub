using System.ComponentModel.DataAnnotations;

namespace PadhaiHub.Models;

public class Question
{
    public int QuestionId { get; set; }

    [Required, StringLength(300)]
    [Display(Name = "Question")]
    public string QuestionText { get; set; } = string.Empty;

    [Required, StringLength(150), Display(Name = "Option A")]
    public string OptionA { get; set; } = string.Empty;

    [Required, StringLength(150), Display(Name = "Option B")]
    public string OptionB { get; set; } = string.Empty;

    [Required, StringLength(150), Display(Name = "Option C")]
    public string OptionC { get; set; } = string.Empty;

    [Required, StringLength(150), Display(Name = "Option D")]
    public string OptionD { get; set; } = string.Empty;

    [Required, StringLength(1)]
    [RegularExpression("^[ABCD]$", ErrorMessage = "Enter A, B, C or D.")]
    [Display(Name = "Correct option")]
    public string CorrectOption { get; set; } = "A";

    [Display(Name = "Quiz")]
    public int QuizId { get; set; }
    public Quiz? Quiz { get; set; }
}