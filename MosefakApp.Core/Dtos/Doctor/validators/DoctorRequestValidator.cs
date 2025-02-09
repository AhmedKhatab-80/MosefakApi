namespace MosefakApp.Core.Dtos.Doctor.validators
{
    public class DoctorRequestValidator : AbstractValidator<DoctorRequest>
    {
        public DoctorRequestValidator()
        {
            RuleFor(x => x.AppUserId)
           .GreaterThanOrEqualTo(0)
           .WithMessage("User Id must be greater than or equal to 0.");

            RuleFor(x => x.YearOfExperience)
           .GreaterThanOrEqualTo(0)
           .WithMessage("Year Of Experience must be greater than or equal to 0.");

            RuleFor(x => x.LicenseNumber)
                .NotEmpty()
                .WithMessage("License Number is required.");

            RuleFor(x => x.AboutMe)
                .NotEmpty()
                .WithMessage("About Me is required.")
                .MaximumLength(500).WithMessage("About Me cannot exceed 500 characters.");

            RuleFor(x => x.ClinicAddresses)
                .NotNull()
                .WithMessage("ClinicAddresses cannot be null.")
                .Must(clinicAddresses => clinicAddresses.Count > 0)
                .WithMessage("At least one ClinicAddress is required.");

            RuleFor(x => x.Specializations)
                .NotNull()
                .WithMessage("Specializations cannot be null.")
                .Must(specializations => specializations.Count > 0)
                .WithMessage("At least one Specialization is required.");

            RuleFor(x => x.WorkingTimes)
                .NotNull()
                .WithMessage("WorkingTimes cannot be null.")
                .Must(workingTimes => workingTimes.Count > 0)
                .WithMessage("At least one WorkingTime is required.");

            RuleFor(x => x.AppointmentTypes)
                .NotNull()
                .WithMessage("AppointmentTypes cannot be null.")
                .Must(appointmentTypes => appointmentTypes.Count > 0)
                .WithMessage("At least one AppointmentType is required.");
        }
    }
}
