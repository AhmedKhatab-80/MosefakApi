namespace MosefakApp.Core.Dtos.User.Requests
{
    public class UserRequest
    {
        [Required]
        [RegularExpression(RegexPatterns.NamePattern, ErrorMessage = Errors.NameRegExp)]
        public string FirstName { get; set; } = null!;

        [Required]
        [RegularExpression(RegexPatterns.NamePattern, ErrorMessage = Errors.NameRegExp)]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(8, ErrorMessage = Errors.PasswordMinLength)]
        [RegularExpression(RegexPatterns.PasswordPattern, ErrorMessage = Errors.PasswordRegExp)]
        public string Password { get; set; } = null!;
        public IList<string> Roles { get; set; } = null!;
    }
}
