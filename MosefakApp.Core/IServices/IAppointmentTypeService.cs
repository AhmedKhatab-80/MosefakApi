namespace MosefakApp.Core.IServices
{
    public interface IAppointmentTypeService 
    {
        Task<List<AppointmentTypeResponse>> GetAppointmentTypes(int doctorId);

    }
}
