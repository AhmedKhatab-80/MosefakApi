namespace MosefakApi.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        public Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTime newDateTime)
        {
            throw new NotImplementedException();
        }
    }
}
