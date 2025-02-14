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
        public const string ViewAvailableTimeSlots = "Permissions.Doctors.ViewAvailableTimeSlots";
        public const string ViewAppointmentTypes = "Permissions.Doctors.ViewAppointmentTypes";
        public const string ViewUpcomingAppointmentsForDoctor = "Permissions.Doctors.ViewUpcomingAppointmentsForDoctor";
        public const string ViewPastAppointmentsForDoctor = "Permissions.Doctors.ViewPastAppointmentsForDoctor";
        public const string GetTotalAppointmentsAsync = "Permissions.Doctors.GetTotalAppointmentsAsync";
        public const string CreateDoctor = "Permissions.Doctors.Create";
        public const string UploadDoctorProfileImage = "Permissions.Doctors.uploadImage";
        public const string CompleteDoctorProfile = "Permissions.Doctors.CompleteDoctorProfile";
        public const string EditDoctor = "Permissions.Doctors.Edit";
        public const string EditDoctorProfile = "Permissions.Doctors.EditDoctorProfile";
        public const string DeleteDoctor = "Permissions.Doctors.Delete";

        public const string ViewUpcomingAppointments = "Permissions.Appointments.ViewUpcomingAppointments";
        public const string ViewCanceledAppointments = "Permissions.Appointments.ViewCanceledAppointments";
        public const string ViewCompletedAppointments = "Permissions.Appointments.ViewCompletedAppointments";
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
        public static IList<string> GetPermissions()
        {
            return typeof(Permissions).GetFields().Select(f => f.GetValue(f) as string).ToList()!;
        }
    }
}
