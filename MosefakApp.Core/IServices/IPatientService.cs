namespace MosefakApp.Core.IServices
{
    public interface IPatientService
    {
        Task<UserProfileResponse?> PatientProfile(int userIdFromClaims);
        Task<UserProfileResponse> UpdatePatientProfile(int userIdFromClaims, UpdatePatientProfileRequest request, CancellationToken cancellationToken = default);
        Task<bool> UploadProfileImageAsync(int patientId, IFormFile imageFile, CancellationToken cancellationToken = default); // FromUserClaims
    }
}
