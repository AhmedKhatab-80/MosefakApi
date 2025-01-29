namespace MosefakApp.Infrastructure.Repositories.Non_Generic
{
    public class AppointmentRepositoryAsync : GenericRepositoryAsync<Appointment>, IAppointmentRepositoryAsync
    {
        public AppointmentRepositoryAsync(AppDbContext context) : base(context)
        {
        }
    }
}
