namespace MosefakApp.Core.IServices
{
    public interface IPatientService
    {
        Task<UserProfileResponse?> PatientProfile(int userIdFromClaims);
        Task<UserProfileResponse> UpdatePatientProfile(int userIdFromClaims, UpdatePatientProfileRequest request, CancellationToken cancellationToken = default);
    }
}
