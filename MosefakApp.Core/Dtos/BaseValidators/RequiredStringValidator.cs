namespace MosefakApp.Core.Dtos.BaseValidators
{
    public class RequiredStringValidator<T> : AbstractValidator<T> where T : class
    {
        public RequiredStringValidator(Expression<Func<T, string>> propertySelector, string propertyName)
        {
            RuleFor(propertySelector)
                .NotEmpty().WithMessage($"{propertyName} is required.");
        }
    }
}
