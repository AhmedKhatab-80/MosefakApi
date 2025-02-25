namespace MosefakApp.Core.Dtos.Appointment.Validators
{
    public class BookAppointmentRequestValidator : AbstractValidator<BookAppointmentRequest> // register config FluentValidation
    {
        public BookAppointmentRequestValidator()
        {

            Include(new RequiredStringValidator<BookAppointmentRequest>(x => x.AppointmentTypeId, "AppointmentTypeId"));


            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("Doctor ID is required");

            RuleFor(x => x.StartDate).GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future.");
            
            Include(new RequiredStringValidator<BookAppointmentRequest>(x => x.ProblemDescription, "Problem Description"));
        }
    }
}
