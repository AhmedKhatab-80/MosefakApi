namespace MosefakApp.Core.Dtos.Award.Validators
{
    public class AwardRequestValidator : AbstractValidator<AwardRequest>
    {
        public AwardRequestValidator()
        {
            Include(new RequiredStringValidator<AwardRequest>(x => x.Title, "Title"));
            Include(new RequiredStringValidator<AwardRequest>(x => x.Organization, "Organization"));

            RuleFor(x => x.DateReceived).NotNull().WithMessage("Date is required");
        }
    }
}
