namespace MosefakApp.Core.Dtos.Schedule.validators
{
    public class WorkingTimeRequestValidator : AbstractValidator<WorkingTimeRequest>
    {
        public WorkingTimeRequestValidator()
        {
            RuleFor(x => x.DayOfWeek)
                .IsInEnum();

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
