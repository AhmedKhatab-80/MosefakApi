namespace MosefakApp.Core.Dtos.AppointmentType.validators
{
    public class AppointmentTypeRequestValidator : AbstractValidator<AppointmentTypeRequest>
    {
        public AppointmentTypeRequestValidator()
        {

            Include(new RequiredStringValidator<AppointmentTypeRequest>(x => x.VisitType, "Visit Type"));

            RuleFor(x => x.ConsultationFee)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ConsultationFee must be greater than or equal 0!");
            
            RuleFor(x => x.Duration)
                .Must(BePositiveTime)
                .WithMessage("Duration must be a positive value.");
        }

        private bool BePositiveTime(TimeSpan duration)
        {
            return duration > TimeSpan.MinValue;
        }
    }
}
