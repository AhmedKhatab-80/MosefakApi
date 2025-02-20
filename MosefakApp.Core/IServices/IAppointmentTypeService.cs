namespace MosefakApp.Core.IServices
{
    public interface IAppointmentTypeService 
    {
        Task<List<AppointmentTypeResponse>?> GetAppointmentTypes(int doctorId); // FromUserClaims
        Task<bool> AddAppointmentType(int doctorId, AppointmentTypeRequest request); // FromUserClaims
        Task<bool> EditAppointmentType(int appointmentTypeId, AppointmentTypeRequest request);
        Task<bool> DeleteAppointmentType(int appointmentTypeId);
    }
}
