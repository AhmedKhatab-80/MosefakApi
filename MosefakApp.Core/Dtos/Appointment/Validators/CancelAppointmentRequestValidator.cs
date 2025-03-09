namespace MosefakApp.Core.Dtos.Appointment.Validators
{
    public class CancelAppointmentRequestValidator : AbstractValidator<CancelAppointmentRequest>
    {
        public CancelAppointmentRequestValidator()
        {

            // will check only in case patient enter CancelationReason to avoid exception

            RuleFor(x => x.CancelationReason).MaximumLength(500)
                                             .WithMessage("Cancelation Reason cannot exceed 500 characters.")
                                             .When(x => !string.IsNullOrEmpty(x.CancelationReason)); 
        }
    }
}
