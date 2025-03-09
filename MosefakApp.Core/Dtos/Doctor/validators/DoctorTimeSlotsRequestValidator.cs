namespace MosefakApp.Core.Dtos.Doctor.validators
{
    public class DoctorTimeSlotsRequestValidator : AbstractValidator<DoctorTimeSlotsRequest>
    {
        public DoctorTimeSlotsRequestValidator()
        {
            RuleFor(x => x.DoctorId)
               .GreaterThanOrEqualTo(0)
               .WithMessage("doctor Id must be greater than or equal to 0.");

            RuleFor(x => x.Date)
            .NotNull()
            .WithMessage("Date is required.");
        }
    }
}
