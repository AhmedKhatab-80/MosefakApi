namespace MosefakApp.Core.IServices
{
    public interface IDoctorService
    {
        Task<IList<DoctorResponse>> GetAllDoctors();
        Task<DoctorResponse> GetDoctorById(int doctorId);
        Task<DoctorProfileResponse> GetDoctorProfile(int appUserIdFromClaims);
        Task<IList<DoctorDto>> TopTenDoctors();
        Task AddDoctor(DoctorRequest request); // for Admin, must pass appUserId explicitly
        Task CompleteDoctorProfile(int appUserIdFromClaims, CompleteDoctorProfileRequest doctor); // note, for Doctor: appUserId will take it from claims too and this represent doctor row in doctors table to update in his profile.
        Task UpdateDoctorProfile(DoctorProfileUpdateRequest request, int appUserIdFromClaims);
        Task DeleteDoctor(int doctorId);
    }
}

/*
 
Note, there are differece between AddDoctor and CompleteProfile or updateProfile because
someone that responsible about addDoctor is admin while someone that responsible about CompleteProfile or Update it is doctor
so admin did't have AppUserId from claims because it's has different token and must pass it exceplicitly inverse doctor
that has AppUserId from claims and didn't need to pass it explicitly...

Admin Actions:

Pass the AppUserId explicitly in the request when managing doctors.
Example use cases:
1-Adding a doctor.
2-Editing doctor-specific fields.
3-Viewing a list of doctors.

User Actions:

Use the AppUserId from (User.Claims) to ensure the action is tied to the logged-in user.
Example use cases:
1-Completing a doctor profile action.
2-Viewing their own doctor profile.
3-updating their own doctor profile.
 
 */
