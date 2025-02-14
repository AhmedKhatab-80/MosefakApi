namespace MosefakApp.Core.Dtos.Schedule.validators
{
    public class WorkingTimeRequestValidator : AbstractValidator<WorkingTimeRequest>
    {
        public WorkingTimeRequestValidator()
        {
            RuleFor(x => x.Day)
                .IsInEnum();

        }
    }
}
