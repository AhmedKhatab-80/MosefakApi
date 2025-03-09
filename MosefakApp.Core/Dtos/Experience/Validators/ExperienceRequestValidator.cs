namespace MosefakApp.Core.Dtos.Experience.Validators
{
    public class ExperienceRequestValidator : AbstractValidator<ExperienceRequest>
    {
        public ExperienceRequestValidator()
        {
            Include(new RequiredStringValidator<ExperienceRequest>(x => x.Title, "Title"));
            Include(new RequiredStringValidator<ExperienceRequest>(x => x.Title, "HospitalName"));
            Include(new RequiredStringValidator<ExperienceRequest>(x => x.Title, "Location"));
            Include(new RequiredStringValidator<ExperienceRequest>(x => x.Title, "Title"));

            RuleFor(x => x.EmploymentType)
                .IsInEnum();

            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage("Start Date can't be null");

            RuleFor(x => x.EndDate)
                .Must((exp,endDate)=> exp.CurrentlyWorkingHere ? endDate != null : endDate == null)
                .WithMessage("End Date should be null if CurrentlyWorkingHere is true, and required otherwise");
        }
    }
}
