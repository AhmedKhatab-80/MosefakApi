namespace MosefakApp.Core.IRepositories.Non_Generic
{
    public interface IAppointmentRepositoryAsync : IGenericRepositoryAsync<Appointment>
    {
        Task<IEnumerable<AppointmentResponse>> GetAppointments(Expression<Func<Appointment,bool>> expression); // I had to add it for better performance
        Task<bool> IsTimeSlotAvailable(int doctorId, DateTime startDate, DateTime endDate);
    }
}
