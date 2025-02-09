namespace MosefakApp.Core.Dtos.Appointment.Validators
{
    public class BookAppointmentRequestValidator : AbstractValidator<BookAppointmentRequest> // register config FluentValidation
    {
        public BookAppointmentRequestValidator()
        {
            RuleFor(x => x.AppointmentTypeId)
                .GreaterThan(0).WithMessage("Appointment Type Id must be valid.");

            RuleFor(x => x.DoctorId)
                .GreaterThan(0).WithMessage("Doctor ID must be valid.");

            RuleFor(x => x.StartDate).GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future.");
            
            Include(new RequiredStringValidator<BookAppointmentRequest>(x => x.ProblemDescription, "Problem Description"));
        }
    }
}
