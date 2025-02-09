namespace MosefakApp.API.Extensions
{
    public static class FluentValidationExtension
    {
        public static IServiceCollection RegisterFluentValidationSettings(this IServiceCollection services)
        {
            // ✅ Register FluentValidation

            services.AddValidatorsFromAssemblyContaining<BookAppointmentRequestValidator>(); // Auto-registers validators
            services.AddFluentValidationAutoValidation(); // Enables automatic validation on request models
            services.AddFluentValidationClientsideAdapters(); // (Optional) Enables client-side validation support

            return services;
        }
    }
}
