namespace MosefakApp.Infrastructure.Identity.Seeding
{
    public static class SeedIdentityData
    {
        public static async Task SeedRolesAndPermissionsAsync(RoleManager<AppRole> roleManager)
        {
            // 1) Ensure roles exist
            var roles = new List<string> { "Admin", "Doctor", "Patient" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new AppRole(role));
                }
            }

            // 2) Assign ALL Permissions from your 'Permissions' static class 
            var rolePermissions = new Dictionary<string, List<string>>
        {
            {
                // =================================================================================
                // =============================== ADMIN PERMISSIONS ===============================
                // =================================================================================
                "Admin", new List<string>
                {
                    // ================== USERS ==================
                    Permissions.Users.View,
                    Permissions.Users.ViewById,
                    Permissions.Users.Create,
                    Permissions.Users.Edit,
                    Permissions.Users.Delete,
                    Permissions.Users.UnLock,

                    // ================== ROLES ==================
                    Permissions.Roles.View,
                    Permissions.Roles.ViewById,
                    Permissions.Roles.Create,
                    Permissions.Roles.Edit,
                    Permissions.Roles.Delete,
                    Permissions.Roles.AssignPermissionToRole,

                    // ================== DOCTORS ==================
                    Permissions.Doctors.View,
                    Permissions.Doctors.ViewById,
                    Permissions.Doctors.ViewProfile,
                    Permissions.Doctors.ViewTopTen,
                    Permissions.Doctors.Search,
                    Permissions.Doctors.ViewAvailableTimeSlots,
                    Permissions.Doctors.ViewUpcomingAppointments,
                    Permissions.Doctors.ViewPastAppointments,
                    Permissions.Doctors.GetTotalAppointments,
                    Permissions.Doctors.Create,
                    Permissions.Doctors.UploadProfileImage,
                    Permissions.Doctors.CompleteProfile,
                    Permissions.Doctors.Edit,
                    Permissions.Doctors.EditProfile,
                    Permissions.Doctors.Delete,
                    Permissions.Doctors.ViewReviews,
                    Permissions.Doctors.ViewAverageRating,
                    Permissions.Doctors.ViewTotalPatientsServed,
                    Permissions.Doctors.ViewEarningsReport,
                    Permissions.Doctors.UpdateWorkingTimesAsync, // Added

                    // ================== APPOINTMENT TYPES ==================
                    Permissions.AppointmentTypes.View,
                    Permissions.AppointmentTypes.Add,
                    Permissions.AppointmentTypes.Edit,
                    Permissions.AppointmentTypes.Delete,

                    // ================== APPOINTMENTS ==================
                    Permissions.Appointments.ViewPatientAppointments,
                    Permissions.Appointments.ViewDoctorAppointments,
                    Permissions.Appointments.ViewPendingForDoctor,
                    Permissions.Appointments.ViewInRangeForDoctor,
                    Permissions.Appointments.View,
                    Permissions.Appointments.ViewInRange,
                    Permissions.Appointments.ViewStatus,
                    Permissions.Appointments.CancelByDoctor,
                    Permissions.Appointments.CancelByPatient,
                    Permissions.Appointments.Approve,
                    Permissions.Appointments.Reject,
                    Permissions.Appointments.MarkAsCompleted,
                    Permissions.Appointments.Reschedule,
                    Permissions.Appointments.Book,
                    Permissions.Appointments.CreatePaymentIntent,
                    Permissions.Appointments.ConfirmPayment,

                    // ================== PATIENTS ==================
                    Permissions.Patients.ViewProfile,
                    Permissions.Patients.EditProfile,
                    Permissions.Patients.UploadProfileImage,

                    // ================== SPECIALIZATIONS ==================
                    Permissions.Specializations.View,
                    Permissions.Specializations.Create,
                    Permissions.Specializations.Edit,
                    Permissions.Specializations.Remove,

                    // ================== EXPERIENCES ==================
                    Permissions.Experiences.View,
                    Permissions.Experiences.Create,
                    Permissions.Experiences.Edit,
                    Permissions.Experiences.Remove,

                    // ================== AWARDS ==================
                    Permissions.Awards.View,
                    Permissions.Awards.Create,
                    Permissions.Awards.Edit,
                    Permissions.Awards.Remove,

                    // ================== EDUCATIONS ==================
                    Permissions.Educations.View,
                    Permissions.Educations.Create,
                    Permissions.Educations.Edit,
                    Permissions.Educations.Remove,

                    // ================== CLINICS ==================
                    Permissions.Clinics.View,
                    Permissions.Clinics.Create,
                    Permissions.Clinics.Edit,
                    Permissions.Clinics.Remove,

                    // ================== REVIEWS ==================
                    Permissions.Reviews.View,
                    Permissions.Reviews.Create,
                    Permissions.Reviews.Edit,
                    Permissions.Reviews.Delete,

                    // ================== CONTACTS ==================
                    Permissions.Contacts.View
                }
            },
            {
                // =================================================================================
                // =============================== DOCTOR PERMISSIONS ===============================
                // =================================================================================
                "Doctor", new List<string>
                {
                    // DOCTOR-RELATED
                    Permissions.Doctors.ViewProfile,
                    Permissions.Doctors.ViewById, // so they can see basic data for themselves 
                    Permissions.Doctors.UploadProfileImage,
                    Permissions.Doctors.EditProfile,
                    Permissions.Doctors.CompleteProfile,
                    Permissions.Doctors.UpdateWorkingTimesAsync,
                    Permissions.Doctors.ViewReviews,
                    Permissions.Doctors.ViewAverageRating,
                    Permissions.Doctors.ViewTotalPatientsServed,
                    Permissions.Doctors.ViewEarningsReport,

                    // APPOINTMENT TYPES (for managing their own schedule)
                    Permissions.AppointmentTypes.View,
                    Permissions.AppointmentTypes.Add,
                    Permissions.AppointmentTypes.Edit,
                    Permissions.AppointmentTypes.Delete,

                    // APPOINTMENTS
                    Permissions.Appointments.ViewDoctorAppointments,
                    Permissions.Appointments.ViewPendingForDoctor,
                    Permissions.Appointments.ViewInRangeForDoctor,
                    Permissions.Appointments.View, // to see details of an appointment
                    Permissions.Appointments.ViewStatus,
                    Permissions.Appointments.CancelByDoctor,
                    Permissions.Appointments.Approve,
                    Permissions.Appointments.Reject,
                    Permissions.Appointments.MarkAsCompleted,
                    Permissions.Appointments.Reschedule,

                    // SPECIALIZATIONS (they can manage their own specialties)
                    Permissions.Specializations.View,
                    Permissions.Specializations.Create,
                    Permissions.Specializations.Edit,
                    Permissions.Specializations.Remove,

                    // EXPERIENCES
                    Permissions.Experiences.View,
                    Permissions.Experiences.Create,
                    Permissions.Experiences.Edit,
                    Permissions.Experiences.Remove,

                    // AWARDS
                    Permissions.Awards.View,
                    Permissions.Awards.Create,
                    Permissions.Awards.Edit,
                    Permissions.Awards.Remove,

                    // EDUCATIONS
                    Permissions.Educations.View,
                    Permissions.Educations.Create,
                    Permissions.Educations.Edit,
                    Permissions.Educations.Remove,

                    // CLINICS
                    Permissions.Clinics.View,
                    Permissions.Clinics.Create,
                    Permissions.Clinics.Edit,
                    Permissions.Clinics.Remove,

                    // REVIEWS
                    // Can view all reviews relevant to them, but not necessarily create reviews as a "doctor"
                    Permissions.Reviews.View,
                }
            },
            {
                // =================================================================================
                // =============================== PATIENT PERMISSIONS ===============================
                // =================================================================================
                "Patient", new List<string>
                {
                    // PATIENT PROFILE
                    Permissions.Patients.ViewProfile,
                    Permissions.Patients.EditProfile,
                    Permissions.Patients.UploadProfileImage,

                    // APPOINTMENTS
                    Permissions.Appointments.ViewPatientAppointments,
                    Permissions.Appointments.View, // to see appointment details for themselves
                    Permissions.Appointments.Book,
                    Permissions.Appointments.CancelByPatient,
                    Permissions.Appointments.CreatePaymentIntent,
                    Permissions.Appointments.ConfirmPayment,

                    // REVIEWS
                    // Typically, a patient can create & edit their own reviews
                    Permissions.Reviews.View,
                    Permissions.Reviews.Create,
                    Permissions.Reviews.Edit,
                    Permissions.Reviews.Delete
                }
            }
        };

            // 3) Assign the permissions to the roles in the DB
            foreach (var rolePermission in rolePermissions)
            {
                var roleName = rolePermission.Key;
                var permissions = rolePermission.Value;

                var role = await roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var existingClaims = await roleManager.GetClaimsAsync(role);

                    foreach (var permission in permissions)
                    {
                        // If the role is missing a given permission, assign it
                        if (!existingClaims.Any(c => c.Type == Permissions.Type && c.Value == permission))
                        {
                            await roleManager.AddClaimAsync(role, new Claim(Permissions.Type, permission));
                        }
                    }
                }
            }
        }
    }


}
