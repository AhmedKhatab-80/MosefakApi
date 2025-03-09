namespace MosefakApp.Core.Dtos.Appointment.Validators
{
    public class RescheduleAppointmentRequestValidator : AbstractValidator<RescheduleAppointmentRequest>
    {
        public RescheduleAppointmentRequestValidator()
        {

            Include(new RequiredStringValidator<RescheduleAppointmentRequest>(x => x.AppointmentId, "AppointmentId"));


            RuleFor(x => x.selectedDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Selected Date must be Greater Than Or Equal To Day.");
        }
    }
}
