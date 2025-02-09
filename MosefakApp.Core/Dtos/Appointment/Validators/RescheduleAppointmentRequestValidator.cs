namespace MosefakApp.Core.Dtos.Appointment.Validators
{
    public class RescheduleAppointmentRequestValidator : AbstractValidator<RescheduleAppointmentRequest>
    {
        public RescheduleAppointmentRequestValidator()
        {
            RuleFor(x => x.AppointmentId).GreaterThan(0).WithMessage("appointment must greater than 0!");

            RuleFor(x => x.NewDateTime).GreaterThan(DateTime.UtcNow).WithMessage("New DateTime must be in the future.");
        }
    }
}
