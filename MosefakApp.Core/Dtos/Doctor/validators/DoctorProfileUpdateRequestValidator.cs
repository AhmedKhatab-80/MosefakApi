namespace MosefakApp.Core.Dtos.Doctor.validators
{
    public class DoctorProfileUpdateRequestValidator : AbstractValidator<DoctorProfileUpdateRequest>
    {
        public DoctorProfileUpdateRequestValidator()
        {
            Include(new RequiredStringValidator<DoctorProfileUpdateRequest>(x=> x.FirstName,"First Name"));
            Include(new RequiredStringValidator<DoctorProfileUpdateRequest>(x=> x.LastName,"Last Name"));
            Include(new RequiredStringValidator<DoctorProfileUpdateRequest>(x=> x.LicenseNumber, "License Number"));

            // Years Of Experience can equal 0 for fresh doctor
            RuleFor(d=> d.YearsOfExperience).GreaterThanOrEqualTo(0).WithMessage("years of experience must greater than or equal 0");

            RuleFor(x => x.Specializations)
                .NotNull().WithMessage("must enter your Specialization")
                .Must(s=> s.Count>0).WithMessage("At least one specialization is required.");


            RuleFor(x => x.WorkingTimes)
                .NotNull().WithMessage("must enter your WorkingTimes")
                .Must(s => s.Count > 0).WithMessage("At least one WorkingTime is required.");


            RuleFor(x => x.AppointmentTypes)
                .NotNull().WithMessage("must enter your AppointmentTypes")
                .Must(s => s.Count > 0).WithMessage("At least one AppointmentType is required.");

             RuleFor(x => x.ClinicAddresses)
                .NotNull().WithMessage("must enter your ClinicAddresses")
                .Must(s => s.Count > 0).WithMessage("At least one ClinicAddresse is required.");

        }
    }
}
