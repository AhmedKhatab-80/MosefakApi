namespace MosefakApp.Infrastructure.constants
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions";

        public const string ViewUsers = "Permissions.Users.View";
        public const string ViewUserById = "Permissions.Users.ViewUserById";
        public const string CreateUser = "Permissions.Users.Create";
        public const string EditUser = "Permissions.Users.Edit";
        public const string DeleteUser = "Permissions.Users.Delete";
        public const string UnLockUser = "Permissions.Users.UnLock";

        public const string ViewRoles = "Permissions.Roles.View";
        public const string ViewRoleById = "Permissions.Roles.ViewRoleById";
        public const string CreateRole = "Permissions.Roles.Create";
        public const string EditRole = "Permissions.Roles.Edit";
        public const string DeleteRole = "Permissions.Roles.Delete";
        public const string AssignPermissionToRole = "Permissions.Roles.AssignPermissionToRole";

        public const string ViewDoctors = "Permissions.Doctors.View";
        public const string ViewDoctorById = "Permissions.Doctors.ViewDoctorById";
        public const string ViewDoctorProfile = "Permissions.Doctors.ViewDoctorProfile";
        public const string ViewTopTenDoctors = "Permissions.Doctors.ViewTopTenDoctors";
        public const string SearchDoctors = "Permissions.Doctors.SearchDoctors";
        public const string ViewAvailableTimeSlots = "Permissions.Doctors.ViewAvailableTimeSlots";
        public const string ViewUpcomingAppointmentsForDoctor = "Permissions.Doctors.ViewUpcomingAppointmentsForDoctor";
        public const string ViewPastAppointmentsForDoctor = "Permissions.Doctors.ViewPastAppointmentsForDoctor";
        public const string GetTotalAppointmentsAsync = "Permissions.Doctors.GetTotalAppointmentsAsync";
        public const string CreateDoctor = "Permissions.Doctors.Create";
        public const string UploadDoctorProfileImage = "Permissions.Doctors.uploadImage";
        public const string CompleteDoctorProfile = "Permissions.Doctors.CompleteDoctorProfile";
        public const string EditDoctor = "Permissions.Doctors.Edit";
        public const string EditDoctorProfile = "Permissions.Doctors.EditDoctorProfile";
        public const string DeleteDoctor = "Permissions.Doctors.Delete";
        public const string ViewDoctorReviews = "Permissions.Doctor.ViewDoctorReviews";
        public const string ViewAverageRating = "Permissions.Doctor.ViewAverageRating";
        public const string ViewTotalPatientsServed = "Permissions.Doctor.ViewTotalPatientsServed";
        public const string ViewEarningsReport = "Permissions.Doctor.ViewEarningsReport";

        public const string ViewAppointmentTypes = "Permissions.Doctors.ViewAppointmentTypes";
        public const string AddAppointmentTypes = "Permissions.Doctors.AddAppointmentTypes";
        public const string EditAppointmentTypes = "Permissions.Doctors.EditAppointmentTypes";
        public const string DeleteAppointmentTypes = "Permissions.Doctors.DeleteAppointmentTypes";

        public const string ViewPatientAppointments = "Permissions.Appointments.ViewPatientAppointments";
        public const string ViewDoctorAppointments = "Permissions.Appointments.ViewDoctorAppointments";
        public const string ViewPendingAppointmentsForDoctor = "Permissions.Appointments.ViewPendingAppointmentsForDoctor";
        public const string ViewAppointmentsForDoctorInRange = "Permissions.Appointments.ViewAppointmentsForDoctorInRange";
        public const string ViewAppointment = "Permissions.Appointments.ViewAppointment";
        public const string ViewAppointmentsInRange = "Permissions.Appointments.ViewAppointmentsInRange";
        public const string ViewAppointmentStatus = "Permissions.Appointments.ViewAppointmentStatus";
        public const string CancelAppointmentByDoctor = "Permissions.Appointments.CancelAppointmentByDoctor";
        public const string CancelAppointmentByPatient = "Permissions.Appointments.CancelAppointmentByPatient";
        public const string ApproveAppointment = "Permissions.Appointments.ApproveAppointment";
        public const string RejectAppointment = "Permissions.Appointments.RejectAppointment";
        public const string MarkAppointmentAsCompleted = "Permissions.Appointments.MarkAppointmentAsCompleted";
        public const string RescheduleAppointment = "Permissions.Appointments.RescheduleAppointment";
        public const string BookAppointment = "Permissions.Appointments.BookAppointment";
        public const string PayForAppointment = "Permissions.Appointments.PayForAppointment";

        public const string ViewPatientProfile = "Permissions.Patients.ViewProfile";
        public const string EditPatientProfile = "Permissions.Patients.EditPatientProfile";

        public const string CreateSpecialization = "Permissions.Specializations.CreateSpecialization";
        public const string EditSpecialization = "Permissions.Specializations.EditSpecialization";
        public const string RemoveSpecialization = "Permissions.Specializations.RemoveSpecialization";

        public const string CreateExperience = "Permissions.Specializations.CreateExperience";
        public const string EditExperience = "Permissions.Experience.EditExperience";
        public const string RemoveExperience = "Permissions.Experience.RemoveExperience";

        public const string CreateAward = "Permissions.Award.CreateAward";
        public const string EditAward = "Permissions.Award.EditAward";
        public const string RemoveAward = "Permissions.Award.RemoveAward";

        public const string CreateEducation = "Permissions.Education.CreateEducation";
        public const string EditEducation = "Permissions.Education.EditEducation";
        public const string RemoveEducation = "Permissions.Education.RemoveEducation";

        public const string ViewClinics = "Permissions.Clinic.ViewClinics";
        public const string CreateClinic = "Permissions.Clinic.CreateClinic";
        public const string EditClinic = "Permissions.Clinic.EditClinic";
        public const string RemoveClinic = "Permissions.Clinic.RemoveClinic";

        public static IList<string> GetPermissions()
        {
            return typeof(Permissions).GetFields().Select(f => f.GetValue(f) as string).ToList()!;
        }
    }
}
