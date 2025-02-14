namespace MosefakApp.Core.Dtos.Doctor.validators
{
    public class DoctorProfileUpdateRequestValidator : AbstractValidator<DoctorProfileUpdateRequest>
    {
        public DoctorProfileUpdateRequestValidator()
        {
            Include(new RequiredStringValidator<DoctorProfileUpdateRequest>(x=> x.FirstName,"First Name"));
            Include(new RequiredStringValidator<DoctorProfileUpdateRequest>(x=> x.LastName,"Last Name"));
            Include(new RequiredStringValidator<DoctorProfileUpdateRequest>(x=> x.LicenseNumber, "License Number"));

        }
    }
}
