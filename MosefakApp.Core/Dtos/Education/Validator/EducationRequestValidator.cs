namespace MosefakApp.Core.Dtos.Education.Validator
{
    public class EducationRequestValidator : AbstractValidator<EducationRequest>
    {
        public EducationRequestValidator()
        {
            Include(new RequiredStringValidator<EducationRequest>(x => x.Degree, "Degree"));
            Include(new RequiredStringValidator<EducationRequest>(x => x.Major, "Major"));
            Include(new RequiredStringValidator<EducationRequest>(x => x.Location, "Location"));
            Include(new RequiredStringValidator<EducationRequest>(x => x.UniversityName, "UniversityName"));

            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage("Start Date can't be null");

            RuleFor(x => x.EndDate)
                .NotNull()
                .WithMessage("End Date should be null if CurrentlyStudying is true, and required otherwise")
                .When(x => x.CurrentlyStudying != false);

            RuleFor(e => e.EndDate)
                .Must((edu, endDate) => edu.CurrentlyStudying ? endDate == null : endDate != null)
                .WithMessage("End Date should be null if CurrentlyStudying is true, and required otherwise");

        }
    }
}
