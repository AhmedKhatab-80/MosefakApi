namespace MosefakApp.Core.IServices
{
    public interface IAppointmentTypeService 
    {
        Task<(List<AppointmentTypeResponse> responses, int totalPages)> GetAppointmentTypes(int doctorId, int pageNumber = 1, int pageSize = 10); // FromUserClaims
        Task<bool> AddAppointmentType(int doctorId, AppointmentTypeRequest request); // FromUserClaims
        Task<bool> EditAppointmentType(int appointmentTypeId, AppointmentTypeRequest request);
        Task<bool> DeleteAppointmentType(int appointmentTypeId);
    }
}
