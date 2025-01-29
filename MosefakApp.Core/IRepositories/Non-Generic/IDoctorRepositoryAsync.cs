namespace MosefakApp.Core.IRepositories.Non_Generic
{
    public interface IDoctorRepositoryAsync : IGenericRepositoryAsync<Doctor>
    {
        Task<IList<DoctorDto>> GetTopTenDoctors();
        Task<IList<Doctor>> GetDoctors();
        Task<Doctor> GetDoctorById(int doctorId);
        Task<Dictionary<int, (string FullName, string ImagePath)>> GetUserDetailsAsync(List<int> appUserIds);
        Task<Dictionary<int, (string FullName, string ImagePath)>> GetUserDetailsAsync(int appUserIds);
        Task<DoctorProfileResponse> GetDoctorProfile(int appUserIdFromClaims);
    }
}
