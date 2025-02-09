namespace MosefakApp.Core.IRepositories.Non_Generic
{
    public interface IDoctorRepositoryAsync : IGenericRepositoryAsync<Doctor>
    {
        Task<List<DoctorDto>?> GetTopTenDoctors();
        Task<List<Doctor>> GetDoctors();
        Task<Doctor> GetDoctorById(int doctorId);
        Task<Dictionary<int, (string FullName, string ImagePath)>> GetUserDetailsAsync(IEnumerable<int> appUserIds);
        Task<Dictionary<int, (string FullName, string ImagePath)>> GetUserDetailsAsync(int appUserIds);
        Task<DoctorProfileResponse> GetDoctorProfile(int appUserIdFromClaims);
        Task<List<DateTime>> GetAvailableTimeSlots(int doctorId, DateTime date, int appointmentTypeId);
        Task<List<AppointmentType>> GetAppointmentTypes(int doctorId); 
    }
}
