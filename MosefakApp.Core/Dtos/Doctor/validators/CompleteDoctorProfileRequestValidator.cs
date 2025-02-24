namespace MosefakApp.Core.Dtos.Doctor.validators
{
    public class CompleteDoctorProfileRequestValidator : AbstractValidator<CompleteDoctorProfileRequest>
    {
        public CompleteDoctorProfileRequestValidator()
        {

            Include(new RequiredStringValidator<CompleteDoctorProfileRequest>(x => x.LicenseNumber, "LicenseNumber"));

            RuleFor(x => x.AboutMe)
                .NotEmpty()
                .WithMessage("About Me is required.")
                .MaximumLength(500).WithMessage("About Me cannot exceed 500 characters.");

            RuleFor(x => x.Clinics)
                .NotNull()
                .WithMessage("Clinics cannot be null.");
                //.Must(clinics => clinics.Count > 0)
                //.WithMessage("At least one Clinic is required.");

            RuleFor(x => x.Specializations)
                .NotNull()
                .WithMessage("Specializations cannot be null.");
                //.Must(specializations => specializations.Count > 0)
                //.WithMessage("At least one Specialization is required.");

            RuleFor(x => x.AppointmentTypes)
                .NotNull()
                .WithMessage("AppointmentTypes cannot be null.");
                //.Must(appointmentTypes => appointmentTypes.Count > 0)
                //.WithMessage("At least one AppointmentType is required.");
        }
    }
}
