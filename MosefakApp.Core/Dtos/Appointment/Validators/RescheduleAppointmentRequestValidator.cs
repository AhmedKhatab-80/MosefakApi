namespace MosefakApp.Core.Dtos.Appointment.Validators
{
    public class RescheduleAppointmentRequestValidator : AbstractValidator<RescheduleAppointmentRequest>
    {
        public RescheduleAppointmentRequestValidator()
        {

            Include(new RequiredStringValidator<RescheduleAppointmentRequest>(x => x.AppointmentId, "AppointmentId"));


            RuleFor(x => x.selectedDate).GreaterThan(DateTime.UtcNow).WithMessage("Selected Date must be in the future.");
        }
    }
}
