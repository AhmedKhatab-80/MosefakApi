namespace MosefakApp.Core.Dtos.Period.validators
{
    public class PeriodRequestValidator : AbstractValidator<PeriodRequest>
    {
        public PeriodRequestValidator()
        {
            RuleFor(x => x.StartTime)
               .NotNull()
               .WithMessage("Start Time is required");


            RuleFor(x => x.EndTime)
                .NotNull()
                .WithMessage("End Time is required");

            RuleFor(x => x)
                .Must(x => x.StartTime < x.EndTime)
                .WithMessage("End time must greater than start time");
        }
    }
}
