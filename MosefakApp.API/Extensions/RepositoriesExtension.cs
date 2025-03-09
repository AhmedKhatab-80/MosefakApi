namespace MosefakApp.API.Extensions
{
    public static class RepositoriesExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDoctorRepositoryAsync, DoctorRepositoryAsync>();
            services.AddScoped<IClinicRepository, ClinicRepository>();
            services.AddScoped<IAppointmentRepositoryAsync, AppointmentRepositoryAsync>();
            services.AddScoped<IPatientRepositoryAsync, PatientRepositoryAsync>();
            services.AddScoped<IUserRepository, UserRepository>();

            // ✅ Add logging to verify repositories before passing to UnitOfWork
            services.AddScoped<IUnitOfWork>(sp =>
            {
                var context = sp.GetRequiredService<AppDbContext>();
                var logger = sp.GetRequiredService<ILoggerService>();

                // Fetch repositories from DI container
                var customRepos = new List<object>
                {
                    sp.GetRequiredService<IDoctorRepositoryAsync>(),
                    sp.GetRequiredService<IClinicRepository>(),
                    sp.GetRequiredService<IAppointmentRepositoryAsync>(),
                    sp.GetRequiredService<IPatientRepositoryAsync>(),
                    sp.GetRequiredService<IUserRepository>(),
                };

                logger.LogInfo("Custom repositories being passed to UnitOfWork: {Repos}",
                    string.Join(", ", customRepos.Select(r => r.GetType().Name)));

                return new UnitOfWork(context, customRepos, logger);
            });

            return services;
        }
    }
}
