namespace MosefakApp.Core.Dtos.User.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [MinLength(8, ErrorMessage = Errors.PasswordMinLength)]
        [Display(Name = "New Password")]
        [RegularExpression(RegexPatterns.PasswordPattern, ErrorMessage = Errors.PasswordRegExp)]
        public string NewPassword { get; set; } = null!;

        [Required]
        [Compare("NewPassword")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
