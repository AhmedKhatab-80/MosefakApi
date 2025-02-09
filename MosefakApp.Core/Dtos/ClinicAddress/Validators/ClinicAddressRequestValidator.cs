namespace MosefakApp.Core.Dtos.ClinicAddress.Validators
{
    public class ClinicAddressRequestValidator : AbstractValidator<ClinicAddressRequest>
    {
        public ClinicAddressRequestValidator()
        {
            Include(new RequiredStringValidator<ClinicAddressRequest>(x => x.Street, "Street"));
            Include(new RequiredStringValidator<ClinicAddressRequest>(x => x.City, "City"));
            Include(new RequiredStringValidator<ClinicAddressRequest>(x => x.Country, "Country"));
        }
    }
}
