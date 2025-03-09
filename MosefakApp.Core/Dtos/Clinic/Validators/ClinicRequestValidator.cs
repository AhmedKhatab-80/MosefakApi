namespace MosefakApp.Core.Dtos.Clinic.Validators
{
    public class ClinicRequestValidator : AbstractValidator<ClinicRequest>
    {
        public ClinicRequestValidator()
        {
            Include(new RequiredStringValidator<ClinicRequest>(x => x.Name, "Name"));
            Include(new RequiredStringValidator<ClinicRequest>(x => x.Street, "Street"));
            Include(new RequiredStringValidator<ClinicRequest>(x => x.City, "City"));
            Include(new RequiredStringValidator<ClinicRequest>(x => x.Country, "Country"));
            Include(new RequiredStringValidator<ClinicRequest>(x => x.Landmark, "Landmark"));
            Include(new RequiredStringValidator<ClinicRequest>(x => x.ApartmentOrSuite, "ApartmentOrSuite"));
            Include(new RequiredStringValidator<ClinicRequest>(x => x.PhoneNumber, "PhoneNumber"));

            RuleFor(x=> x.WorkingTimes)
                .NotNull().WithMessage("Working Times can't be null")
                .Must(x=> x.Count() > 0)
                .WithMessage("at least must have one Working Time");
        }
    }
}
